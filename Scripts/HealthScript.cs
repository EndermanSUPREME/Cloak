using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] float totalHealth = 1;
    float maxHealth;
    public int PointWorth = 10;
    [SerializeField] GameObject DeathParticleSys;
    
    PlayerStats playerStatScript;
    AudioSource DeathSound;
    public bool isBoss = false, isSegment = false, isDragon = false;
    public bool hasBeenInjured = false;
    public Transform BosshealthBar;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color flashColor, fullColor;
    [SerializeField] Rigidbody2D playerRb2D;
    Transform Player;

    void Start()
    {
        maxHealth = totalHealth;
        playerStatScript = GameObject.FindObjectOfType<PlayerStats>();
        Player = GameObject.Find("PlayerSprite").transform;

        if (transform.GetComponent<SpriteRenderer>() != null)
        {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
        }
        
        if (!isBoss)
        {
            DeathSound = GameObject.Find("generalEnemyDeath").transform.GetComponent<AudioSource>();
        } else
            {
                DeathSound = GameObject.Find("BossDeathSound").transform.GetComponent<AudioSource>();
            }
    }

    public void TakeDamage() // AI Gets Damaged By Player
    {
        if (isBoss)
        {
            if (!hasBeenInjured)
            {
                if (!isSegment)
                {
                    // golem & dragon
                    if (playerStatScript.isCounterable())
                    {
                        Vector2 genDir;
    
                        if (Player.position.x > transform.position.x) // player is on the right
                        {
                            genDir = new Vector2(6, 1);
                        } else // player is on the left
                            {
                                genDir = new Vector2(-6, 1);
                            }
    
                        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        Player.GetComponent<PlayerStats>().PlayerDamaged(genDir);
    
                        // print("Player Countered");
    
                        return;
                    } else
                        {
                            playerStatScript.playerCanBeCountered();
                        }

                } else
                    {
                        Invoke("allowDamage", 1); // centipede
                    }

                if (isDragon)
                {
                    Invoke("allowDamage", 3f);
                }
                hasBeenInjured = true;

                totalHealth--;

                if (totalHealth <= 0)
                {
                    if (!isSegment)
                    {
                        dead();
                    } else
                        {
                            destorySegment();
                        }
                    return;
                } else
                    {
                        if (BosshealthBar != null)
                        {
                            float scaleX = (totalHealth/maxHealth);
                            BosshealthBar.localScale = new Vector3(scaleX, 1, 1);
                        }
                    }

                KnockBackPlayer();

                StartCoroutine(spriteFlash());
            }
        } else
            {
                totalHealth--;

                if (totalHealth <= 0)
                    dead();
            }
    }

    void allowDamage()
    {
        hasBeenInjured = false;
    }

    void KnockBackPlayer()
    {
        Vector2 genDir;

        if (Player.position.x > transform.position.x) // player is on the right
        {
            genDir = new Vector2(4, 3);
        } else // player is on the left
        {
            genDir = new Vector2(-4, 3);
        }

        // when player isDamaged bool is true the player cannot control the player
        Player.GetComponent<playerMovement>().isPlayerDamaged(true);
        playerRb2D.velocity = Vector2.zero;
        playerRb2D.gravityScale = 0;
        
        Invoke("reEnablePlayerMovement", 0.5f);

        playerRb2D.AddForce(genDir, ForceMode2D.Impulse);
    }

    void reEnablePlayerMovement()
    {
        // player regains control
        playerRb2D.gravityScale = 1;
        Player.GetComponent<playerMovement>().isPlayerDamaged(false);
    }

    IEnumerator spriteFlash()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.07f);

        spriteRenderer.color = fullColor;

        yield return new WaitForSeconds(0.07f);

        if (hasBeenInjured)
        {
            StartCoroutine(spriteFlash());
        }
    }

    void destorySegment()
    {
        transform.gameObject.SetActive(false);
    }

    public void dead()
    {
        playerStatScript.GainPoints(PointWorth);

        DeathSound.Play();

        if (DeathParticleSys != null)
        {
            GameObject particle = Instantiate(DeathParticleSys, transform.position, DeathParticleSys.transform.rotation);
            Destroy(particle, 2.5f);
        }
        
        Destroy(transform.parent.gameObject);
    }
}//EndScript