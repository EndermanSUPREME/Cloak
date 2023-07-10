using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement_backup : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;
    
    bool lastMovementWasRight, isInputRemoved = false, wallStick = false, isWallSliding = false, hasWallJumped = false, isSliding = false, isAttacking = false;

    float inputX, curSpeed;

    BoxCollider2D mainCollider;
    PolygonCollider2D col;
    SpriteRenderer spriteRenderer;
    List<Vector2> physicsShape = new List<Vector2>();

    public LayerMask groundLayer, wallLayer, EnemyLayer;
    [SerializeField] float speedConst, jumpForce;
    [SerializeField] Vector2 footSize, headSize, landingSize, wallGrabBox;
    [SerializeField] Transform foot, head, MainCamera, leftSide, rightSide;
    [SerializeField] AudioSource JumpSound, AttackSound;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        mainCollider = GetComponent<BoxCollider2D>();
        col = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Application.targetFrameRate = 65;
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");

        if (inputX > 0.15f)
        {
            lastMovementWasRight = true;
        } else if (inputX < -0.15f)
            {
                lastMovementWasRight = true;
            }
    }

    void FixedUpdate()
    {
        isGrounded();
        PlayerInput();
        CameraMotion();
    }

    void CameraMotion()
    {
        MainCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    bool isGrounded()
    {
        if (Physics2D.OverlapBox(foot.position, footSize, 0, groundLayer))
        {
            return true;
        } else
            {
                return false;
            }
    }

    bool isHeadObstructed()
    {
        if (Physics2D.OverlapBox(head.position, headSize, 0, groundLayer))
        {
            return true;
        } else
            {
                return false;
            }
    }

    bool isTouchingWallOnTheLeft()
    {
        if (Physics2D.OverlapBox(leftSide.position, wallGrabBox, 0, wallLayer))
        {
            return true;
        } else
            {
                return false;
            }
    }

    bool isTouchingWallOnTheRight()
    {
        if (Physics2D.OverlapBox(rightSide.position, wallGrabBox, 0, wallLayer))
        {
            return true;
        } else
            {
                return false;
            }
    }

    void LateUpdate()
    {
        if (isSliding && isInputRemoved)
        {
            mainCollider.enabled = false;
            col.enabled = true;

            UpdateCollider();
            playerSlide();
        } else
            {
                mainCollider.enabled = true;
                col.enabled = false;
            }
    }

    void UpdateCollider() // only use when sliding
    {
        spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        col.SetPath(0, physicsShape);
    }

    public void isPlayerDamaged(bool v) // override the input when the player gets damaged
    {
        if (isSliding)
        {
            isSliding = false;
        }

        isInputRemoved = v;
    }

//==================================================================================================================================
//==================================================================================================================================

    void PlayerInput()
    {
        if (!isInputRemoved)
            BasicMovement(inputX);
            PlayerAttack();
    }

    void BasicMovement(float x)
    {
        UpdateAnimator();

        if (!wallStick) // regular movement
        {
            rb2D.velocity = new Vector2(x * speedConst, rb2D.velocity.y);
        } else // player remains still for a moment on the initial wall stick before sliding down or performing a wall jump
            {
                rb2D.velocity = Vector2.zero;
            }

        // Allow Player to Jump/Slide OR Check for free-fall and wall related movements
        if (isGrounded())
        {
            if (!isWallSliding && !wallStick)
            {
                if (Input.GetKeyDown(KeyCode. Space))
                {
                    rb2D.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                } else
                    {
                        anim.SetBool("jumpL", true);
                        anim.SetBool("jumpR", true);
                    }
    
                if (Input.GetKeyDown(KeyCode. S) && !isSliding && Mathf.Abs(rb2D.velocity.x) > 4)
                {
                    // anim web goes to sliding anim and the player can use inputs during a slide
                    isInputRemoved = true;
                    isSliding = true;
                    curSpeed = rb2D.velocity.x;
                }
            }
        } else
            {
                if (!hasWallJumped)
                {
                    if (!isWallSliding) // perform the initial stick
                    {
                        if (isTouchingWallOnTheLeft())
                        {
                            StartCoroutine(InitialWallStick());
                        } else if (isTouchingWallOnTheRight())
                            {
                                StartCoroutine(InitialWallStick());
                            }
                    } else // start the slide and test for jump input to wall jump
                        {
                            if (Input.GetKeyDown(KeyCode. Space))
                            {
                                if (lastMovementWasRight) // sliding on the right so jump to the left
                                {
                                    rb2D.AddForce(new Vector2(-4, 4), ForceMode2D.Impulse);
                                } else // sliding on the left so jump to the right
                                    {
                                        rb2D.AddForce(new Vector2(4, 4), ForceMode2D.Impulse);
                                    }

                                StartCoroutine(WallJumping());
                            } else
                                {
                                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                                }
                        }
                }
            }
    }

    void UpdateAnimator()
    {
        anim.SetFloat("speed", rb2D.velocity.x);
        anim.SetBool("sliding", isSliding);
        anim.SetBool("wallSliding", isWallSliding);

        anim.SetBool("grounded", isGrounded());
        
        if (Input.GetKeyDown(KeyCode. Space))
        {
            if (lastMovementWasRight)
            {
                anim.SetBool("jumpL", true);
                anim.SetBool("jumpR", false);
            } else
                {
                    anim.SetBool("jumpL", false);
                    anim.SetBool("jumpR", true);
                }
        }
    }

    IEnumerator InitialWallStick()
    {
        wallStick = true;
        isWallSliding = false;
        yield return new WaitForSeconds(0.25f);
        wallStick = false;
        isWallSliding = true;
    }

    IEnumerator WallJumping()
    {
        hasWallJumped = true;
        yield return new WaitForSeconds(0.25f);
        hasWallJumped = false;
    }

    void playerSlide()
    {
        if (isHeadObstructed()) // in slide tunnel
        {
            if (lastMovementWasRight) // slide Right
            {
                if (curSpeed < 6)
                {
                    if (rb2D.velocity.x < 6)
                    {
                        rb2D.velocity = new Vector2(rb2D.velocity.x + 0.15f, 0);
                    } else
                        {
                            rb2D.velocity = new Vector2(6, 0);
                        }
                } else
                    {
                        rb2D.velocity = new Vector2(curSpeed, 0);
                    }
            } else // slide Left
                {
                    if (curSpeed > -6)
                    {
                        if (rb2D.velocity.x > -6)
                        {
                            rb2D.velocity = new Vector2(rb2D.velocity.x - 0.15f, 0);
                        } else
                            {
                                rb2D.velocity = new Vector2(-6, 0);
                            }
                    } else
                        {
                            rb2D.velocity = new Vector2(curSpeed, 0);
                        }
                }
        } else // slide to a semi-stop
            {
                if (Mathf.Abs(rb2D.velocity.x) > 4)
                {
                    if (lastMovementWasRight)
                    {
                        rb2D.velocity = new Vector2(rb2D.velocity.x - 0.2f, 0);
                    } else
                        {
                            rb2D.velocity = new Vector2(rb2D.velocity.x + 0.2f, 0);
                        }
                } else
                    {
                        isSliding = false;
                        isInputRemoved = false;
                    }
            }
    }

    void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode. LeftShift))
        {
            if (!isAttacking)
            {
                if (lastMovementWasRight)
                {
                    anim.Play("attackR");
                } else
                    {
                        anim.Play("attackL");
                    }
    
                isAttacking = true;
            }
        }
    }

    public void FormAttackBubble()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 1.15f, EnemyLayer);

        foreach (Collider2D E in nearbyEnemies)
        {
            if (E.transform.GetComponent<HealthScript>() != null)
            {
                E.transform.GetComponent<HealthScript>().TakeDamage();
            }
        }
    }

    public void EndAttack() // player animation event
    {
        isAttacking = false;
    }

//==================================================================================================================================
//==================================================================================================================================

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(foot.position, footSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(head.position, headSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(foot.position, landingSize);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(leftSide.position, wallGrabBox);
        Gizmos.DrawCube(rightSide.position, wallGrabBox);
    }
}//EndScript