using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;
    
    bool lastMovementWasRight, isInputRemoved = false, isSliding = false, isAttacking = false,
        isWallSliding = false, sticking = false, wallJump = false, grabOntoWall = true,
        wallMovement = false, wasHeadObstructedDuringSlide = false, FullTracking = false, ungroundedDelay = true;

    float inputX, curSpeed, slideFriction;

    BoxCollider2D mainCollider;
    [SerializeField] Vector2 regularSize, normalLocation, slideSize, slideLocation;

    public LayerMask groundLayer, wallLayer, EnemyLayer;
    [SerializeField] float speedConst, jumpForce;
    [SerializeField] Vector2 footSize, headSize, landingSize, wallGrabBox;
    [SerializeField] Transform foot, head, MainCamera, leftSide, rightSide;
    [SerializeField] AudioSource JumpSound, AttackSound;

    Vector2 lastPlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        mainCollider = GetComponent<BoxCollider2D>();

        Application.targetFrameRate = 65;
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
        {
            lastMovementWasRight = true;
        } else if (inputX < 0)
            {
                lastMovementWasRight = false;
            }

        if (rb2D.velocity.y > 8)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, 8);
        } else if (rb2D.velocity.y < -8)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, -8);
            }

        PlayerInput();
    }

    bool landedOnEnemy()
    {
        if (Physics2D.OverlapBox(foot.position, landingSize, 0, EnemyLayer))
        {
            return true;
        } else
            {
                return false;
            }
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
        CameraMotion();

        if (isSliding && isInputRemoved && !isAttacking)
        {
            playerSlide();
        } else
            {
                mainCollider.size = regularSize;
                mainCollider.offset = normalLocation;
            }
    }

//==================================================================================================================================
//==================================================================================================================================

    public void isPlayerDamaged(bool v) // override the input when the player gets damaged
    {
        if (isSliding)
        {
            isSliding = false;
        }

        isInputRemoved = v;
    }

    void CameraMotion()
    {
        // MainCamera.position = new Vector3(transform.position.x, transform.position.y, -10); // For Testing Purposes

        if (!FullTracking)
        {
            // follow the player horizontally
            MainCamera.position = new Vector3(transform.position.x, MainCamera.position.y, -10);

            // is check whether the player is too high or low on the screen and adjust the MainCamera.position.y
            Vector2 playerPosition2D, cameraPosition2D;
            playerPosition2D = new Vector2(transform.position.x, transform.position.y);
            cameraPosition2D = new Vector2(MainCamera.position.x, MainCamera.position.y);

            if (Vector2.Distance(playerPosition2D, cameraPosition2D) > 2.35f)
            {
                FullTracking = true;
            }
        } else // have the camera move towards the center of the player and stay locked on until we're grounded
            {
                Vector3 centerOfThePlayer = new Vector3(transform.position.x, transform.position.y, -10);
                float cameraRepositionSpeed = 15;

                if (!isGrounded())
                {
                    if (MainCamera.position != centerOfThePlayer) // reposition
                    {
                        MainCamera.position = Vector3.MoveTowards(MainCamera.position, centerOfThePlayer, cameraRepositionSpeed * Time.deltaTime);
                    } else // stick and remain until grounded
                        {
                            MainCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
                        }
                } else
                    {
                        if (MainCamera.position != centerOfThePlayer) // reposition
                        {
                            MainCamera.position = Vector3.MoveTowards(MainCamera.position, centerOfThePlayer, cameraRepositionSpeed * Time.deltaTime);
                        } else
                            {
                                FullTracking = false;
                            }
                    }
            }
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

        if (!sticking)
        {
            rb2D.gravityScale = 1;

            if (!wallMovement)
            {
                rb2D.velocity = new Vector2(x * speedConst, rb2D.velocity.y);
            }
        } else // sticking to the wall
            {
                rb2D.velocity = Vector2.zero;
                rb2D.gravityScale = 0;
            }

        // Allow Player to Jump/Slide OR Check for free-fall and wall related movements
        if (isGrounded())
        {
            ungroundedDelay = true;
            wallMovement = false;
            grabOntoWall = true;
            wallJump = false;
            sticking = false;
            isWallSliding = false;

            if (transform.GetComponent<PlayerStats>() != null)
            {
                if (transform.GetComponent<PlayerStats>().isCounterable()) // checks if we can be countered by the boss and reverts status (player cant just go crazy on the bosses
                {
                    transform.GetComponent<PlayerStats>().revertCounter();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isAttacking = false;

                rb2D.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.S) && !isSliding && Mathf.Abs(rb2D.velocity.x) > 4 && !isAttacking)
            {
                // anim web goes to sliding anim and the player can use inputs during a slide

                isInputRemoved = true;
                isSliding = true;

                curSpeed = rb2D.velocity.x;

                if (!lastMovementWasRight) // anims were flipped during creation
                {
                    anim.Play("slideR");
                } else
                    {
                        anim.Play("slideL");
                    }
            }
        } else // NOT GROUNDED
            {
                if (!ungroundedDelay)
                {
                    if (!isAttacking)
                    {
                        if (grabOntoWall)
                        {
                            // check for wall
                            if (isTouchingWallOnTheLeft())
                            {
                                lastMovementWasRight = false;
                                StartCoroutine(InitialWallStick());
                                // anim.Play("wallSlideL");
                            } else if (isTouchingWallOnTheRight())
                                {
                                    lastMovementWasRight = true;
                                    StartCoroutine(InitialWallStick());
                                    // anim.Play("wallSlideR");
                                }
                        } else
                            {
                                // wall jump
                                if (!wallJump)
                                {
                                    wallMovement = true;
                                    isWallSliding = true;
                                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    
                                    anim.SetBool("wallSliding", isWallSliding);
        
                                    if (Input.GetKeyDown(KeyCode.Space))
                                    {
                                        rb2D.velocity = Vector2.zero;
                                        Vector2 wallJumpDir;
        
                                        if (anim.GetCurrentAnimatorStateInfo(0).IsName("wallSlideL"))
                                        {
                                            wallJumpDir = new Vector2(6.5f,6.5f);
                                            rb2D.AddForce(wallJumpDir, ForceMode2D.Impulse);
                                        } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("wallSlideR"))
                                            {
                                                wallJumpDir = new Vector2(-6.5f,6.5f);
                                                rb2D.AddForce(wallJumpDir, ForceMode2D.Impulse);
                                            }
        
                                        wallJump = true;
                                        sticking = false;
                                        isWallSliding = false;
        
                                        // Debug.Log(rb2D.velocity);
        
                                        Invoke("ResetWallFunctions", 0.225f);
                                    }
                                }
                            }
                    }
                } else
                    {
                        Invoke("delayWallMovement", 0.35f);
                    }
            }
    }

    void delayWallMovement()
    {
        ungroundedDelay = false;
    }

    void ResetWallFunctions()
    {
        wallMovement = false;

        grabOntoWall = true;
        wallJump = false;
        sticking = false;
        isWallSliding = false;
    }

    IEnumerator InitialWallStick()
    {
        grabOntoWall = false;

        wallJump = false;
        sticking = true;
        isWallSliding = false;

        if (isTouchingWallOnTheLeft())
        {
            anim.Play("wallSlideL");
        }
        else if (isTouchingWallOnTheRight())
        {
            anim.Play("wallSlideR");
        }
        
        yield return new WaitForSeconds(0.65f);
        sticking = false;
        isWallSliding = true;
    }

    void UpdateAnimator()
    {
        if (isGrounded() && inputX == 0 && !isAttacking)
        {
            if (lastMovementWasRight)
            {
                anim.Play("idleRight");
            } else
                {
                    anim.Play("idleLeft");
                }
        }

        anim.SetFloat("speed", inputX * speedConst);
        anim.SetBool("sliding", isSliding);
        anim.SetBool("wallSliding", isWallSliding);

        anim.SetBool("grounded", isGrounded());
        
        if (Input.GetKeyDown(KeyCode. Space) && isGrounded())
        {
            if (lastMovementWasRight)
            {
                anim.Play("jumpRight");
            } else
                {
                    anim.Play("jumpLeft");
                }
        }

        if (!isTouchingWallOnTheLeft() && !isTouchingWallOnTheRight())
        {
            if (!isGrounded() && !isAttacking)
            {
                if (lastMovementWasRight)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jumpSpinRight"))
                    {
                        anim.Play("jumpSpinRight");
                    }
                }
                else
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jumpSpinLeft"))
                    {
                        anim.Play("jumpSpinLeft");
                    }
                }
            }
        }
    }

    void playerSlide()
    {
        anim.SetBool("sliding", isSliding);
    
        mainCollider.size = slideSize;
        mainCollider.offset = slideLocation;

        if (wasHeadObstructedDuringSlide)
        {
            slideFriction = 0.3f;
        } else
            {
                slideFriction = 0.1f;
            }

        if (isHeadObstructed()) // in slide tunnel
        {
            isSliding = true;
            isInputRemoved = true;

            wasHeadObstructedDuringSlide = true;

            Debug.Log("Head Obstructed -> Sliding is " + isSliding);

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
                StartCoroutine(TrackPosition());

                if (rb2D.velocity.x > 4)
                {
                    rb2D.velocity = new Vector2(rb2D.velocity.x - slideFriction, 0);

                    if (rb2D.velocity.x <= 4)
                    {
                        wasHeadObstructedDuringSlide = false;
                        isSliding = false;
                        isInputRemoved = false;
                    }

                    Debug.Log("Sliding Right To A Stop");
                } else if (rb2D.velocity.x < -4)
                    {
                        rb2D.velocity = new Vector2(rb2D.velocity.x + slideFriction, 0);

                        if (rb2D.velocity.x >= -4)
                        {
                            wasHeadObstructedDuringSlide = false;
                            isSliding = false;
                            isInputRemoved = false;
                        }

                        Debug.Log("Sliding Left To A Stop");
                    } else if (rb2D.velocity.x == 0)
                        {
                            wasHeadObstructedDuringSlide = false;
                            isSliding = false;
                            isInputRemoved = false;

                            Debug.Log("Slided Into A Wall");
                        } else if (lastMovementWasRight && (rb2D.velocity.x > 0 && rb2D.velocity.x < 3))
                            {
                                if (Vector2.Distance((Vector2) transform.position, lastPlayerPos) == 0)
                                {
                                    wasHeadObstructedDuringSlide = false;
                                    isSliding = false;
                                    isInputRemoved = false;

                                    Debug.Log(Vector2.Distance((Vector2) transform.position, lastPlayerPos));

                                    Debug.Log("Right Slide Stuck but no movement occured");
                                }
                            } else if (!lastMovementWasRight && (rb2D.velocity.x < 0 && rb2D.velocity.x > -3))
                                {
                                    if (Vector2.Distance((Vector2) transform.position, lastPlayerPos) == 0)
                                    {
                                        wasHeadObstructedDuringSlide = false;
                                        isSliding = false;
                                        isInputRemoved = false;

                                        Debug.Log(Vector2.Distance((Vector2) transform.position, lastPlayerPos));

                                        Debug.Log("Left Slide Stuck but no movement occured");
                                    }
                                }
            }
    }

    IEnumerator TrackPosition()
    {
        lastPlayerPos = (Vector2) transform.position;
        yield return new WaitForSeconds(0.1f);
        if (isSliding)
        {
            StartCoroutine(TrackPosition());
        }
    }

    void PlayerAttack()
    {
        // Debug.Log(isAttacking);

        if (!isGrounded() && landedOnEnemy() && !wallMovement && !isSliding)
        {
            Debug.Log("Landed On Enemy == " + Physics2D.OverlapBoxAll(foot.position, landingSize, 0, EnemyLayer)[0].transform);
            
            if (Physics2D.OverlapBoxAll(foot.position, landingSize, 0, EnemyLayer)[0].transform.GetComponent<HealthScript>() != null)
            {
                Physics2D.OverlapBoxAll(foot.position, landingSize, 0, EnemyLayer)[0].transform.GetComponent<HealthScript>().TakeDamage();
            }
        }

        if (Input.GetKeyDown(KeyCode. LeftShift) || Input.GetKeyDown(KeyCode. RightShift))
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