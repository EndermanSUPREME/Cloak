using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class golemBossScript : MonoBehaviour
{
    bool startFighting = false, iFrame = false, isDamaged = false;
    Animator anim;
    Rigidbody2D rb2D;

    Transform dest, PlayerObject;
    [SerializeField] Transform pointA, pointB, rayPoint;

    [SerializeField] HealthScript healthScript;
    [SerializeField] LayerMask playerLayer;
    protected string attackAnimName;

    public void allowedToMove()
    {
        if (!startFighting)
        {
            startFighting = true;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        PlayerObject = GameObject.Find("PlayerSprite").transform;

        dest = pointA;
    }

    void Update()
    {
        if (startFighting)
        {
            CheckDamageStatus();
    
            if (!isDamaged)
            {
                LoopPathing();

                GolemAttack();
            }
        }
    }

    void CheckDamageStatus()
    {
        if (healthScript.hasBeenInjured)
        {
            if (!iFrame)
            {
                I_Frame();
            }

            iFrame = true;
        }
    }

    void golemTurnAround()
    {
        healthScript.hasBeenInjured = false;
        iFrame = false;

        isDamaged = false;
    }

    void I_Frame()
    {
        isDamaged = true;
        Invoke("golemTurnAround", 1);
    }

    void LoopPathing()
    {
        if (dest != null)
        {
            if (Vector2.Distance(dest.position, transform.position) < 0.05f)
            {
                if (dest == pointA)
                {
                    dest = pointB;
                } else
                    {
                        dest = pointA;
                    }

                return;
            }

            anim.SetFloat("speed", rb2D.velocity.x);
        }
    }

    void GolemAttack()
    {
        if (dest == pointA)
        {
            rb2D.velocity = new Vector2(-1, 0);

            attackAnimName = "attackLeft";
            AttackRaycast(-transform.right);
        } else if (dest == pointB)
            {
                rb2D.velocity = new Vector2(1, 0);

                attackAnimName = "attackRight";
                AttackRaycast(transform.right);
            }
    }

    void AttackRaycast(Vector2 attackDir)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rayPoint.position, attackDir, 1.15f, playerLayer);

        if (hit2D.collider != null && !iFrame)
        {
            if (hit2D.collider.transform.GetComponent<PlayerStats>() != null)
            {
                print("Player Hit");

                anim.Play(attackAnimName);

                Vector2 genDir;

                if (PlayerObject.position.x > transform.position.x) // player is on the right
                {
                    genDir = new Vector2(6, 1);
                } else // player is on the left
                    {
                        genDir = new Vector2(-6, 1);
                    }

                PlayerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                hit2D.collider.transform.GetComponent<PlayerStats>().PlayerDamaged(genDir);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.transform == PlayerObject && !iFrame)
        {
            // collision with player
            Vector2 genDir;

            if (PlayerObject.position.x > transform.position.x) // player is on the right
            {
                genDir = new Vector2(6, 1);
            } else // player is on the left
                {
                    genDir = new Vector2(-6, 1);
                }

            PlayerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            PlayerObject.GetComponent<PlayerStats>().PlayerDamaged(genDir);

            // // print(genDir + " [==] " + PlayerObject.GetComponent<Rigidbody2D>().velocity);
        }
    }

}//EndScript