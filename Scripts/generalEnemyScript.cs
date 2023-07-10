using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalEnemyScript : MonoBehaviour
{
    [SerializeField] Transform PointA, PointB;
    Transform destinationPoint, Player;
    [SerializeField] float speed;
    Rigidbody2D rb2D;
    Animator anim;

    SpriteRenderer spriteRenderer;
    List<Vector2> physicsShape = new List<Vector2>();

    public bool canAttack = false;
    bool isAttacking = false;
    public LayerMask playerLayer;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        Player = GameObject.Find("PlayerSprite").transform;

        if (Random.Range(0, 100) % 4 == 0)
        {
            destinationPoint = PointB;
        } else
            {
                destinationPoint = PointA;
            }
    }

    void Update()
    {
        LoopMovement();
    }

    void EnemyAttack()
    {
        RaycastHit2D hit2D;

        if (destinationPoint == PointB)
        {
            hit2D = Physics2D.Raycast(transform.position, transform.right, 1.05f, playerLayer);
            if (hit2D.collider != null)
            {
                isAttacking = true;

                // anim.Play("attackRight");
                AttackRaycast(transform.right);
            }
        } else
            {
                hit2D = Physics2D.Raycast(transform.position, -transform.right, 1.05f, playerLayer);
                
                if (hit2D.collider != null)
                {
                    isAttacking = true;

                    // anim.Play("attackLeft");
                    AttackRaycast(-transform.right);
                }
            }
    }

    void AttackRaycast(Vector2 attackDir)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, attackDir, 1.05f, playerLayer);

        if (hit2D.collider != null)
        {
            if (hit2D.collider.transform.GetComponent<PlayerStats>() != null)
            {
                print("Player Hit");

                Vector2 genDir;

                if (hit2D.collider.transform.position.x > transform.position.x) // player is on the right
                {
                    genDir = new Vector2(2, 1);
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attackRight"))
                    {
                        anim.Play("attackRight");
                    }
                } else // player is on the left
                    {
                        genDir = new Vector2(-2, 1);
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attackLeft"))
                        {
                            anim.Play("attackLeft");
                        }
                    }

                hit2D.collider.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                hit2D.collider.transform.GetComponent<PlayerStats>().PlayerDamaged(genDir);
            }
        }
    }

    void attackFinished() // animation event
    {
        if (Vector2.Distance(Player.position, transform.position) > 1)
        {
            isAttacking = false;
        }
    }

    void LoopMovement()
    {
        if (canAttack)
        {
            EnemyAttack();
        }

        if (Vector2.Distance(transform.position, destinationPoint.position) < 0.05f)
        {
            if (destinationPoint == PointA)
            {
                destinationPoint = PointB;
            } else
                {
                    destinationPoint = PointA;
                }
        }

        if (!isAttacking)
        {
            if (destinationPoint == PointA) // move Left
            {
                rb2D.velocity = new Vector2(-speed, rb2D.velocity.y);
            } else // move Right
                {
                    rb2D.velocity = new Vector2(speed, rb2D.velocity.y);
                }
        } else
            {
                rb2D.velocity = Vector2.zero;
            }

        anim.SetFloat("speed", rb2D.velocity.x);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.transform == Player)
        {
            Vector2 genDir;

            if (Player.position.x > transform.position.x) // player is on the right
            {
                genDir = new Vector2(4, 3);
            } else // player is on the left
                {
                    genDir = new Vector2(-4, 3);
                }

            Player.GetComponent<PlayerStats>().PlayerDamaged(genDir + new Vector2(0, 1.5f));
        }
    }
}//EndScript