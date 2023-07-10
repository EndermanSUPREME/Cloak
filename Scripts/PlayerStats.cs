using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    int totalPoints = 0, aurasCollected = 0, totalLives = 3, nextLifeinc = 1, FinalScore = 0;
    float currentTime = 0;
    [SerializeField] GameObject AuraObject;
    Vector2[] dropDirections = {new Vector2(-0.5f, -0.5f), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(0.5f, 0.5f)};
    [SerializeField] AudioSource AuraCollectedSound, AuroLostSound;

    [SerializeField] Text AuraDisplay, LifeDisplay, PointsDisplay, TimeDisplay;
    [SerializeField] Animator playerAnim, GameOverScreenAnim;
    bool isDead = false, playerIFrame = false, counterable = false, timeStop = false, finalScoreDynamic = false;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color flashColor, fullColor;

    [SerializeField] playerMovement PlayerMovementScript;
    MenuScript screenScript;

    void Start()
    {
        screenScript = GameObject.FindObjectOfType<MenuScript>();

        if (PlayerPrefs.GetInt("totalLives") > 0)
        {
            totalLives = PlayerPrefs.GetInt("totalLives");
        } else // player runs out of lives
            {
                totalLives = 3;
                PlayerPrefs.SetInt("totalLives", totalLives);
            }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            totalLives = 3;
            PlayerPrefs.SetInt("totalLives", totalLives);
        }

        UpdateTotalLives();
        UpdateAuraDisplay();
        PointsDisplay.text = "Score :: " + totalPoints.ToString();
        nextLifeinc = 1;
    }

    void Update()
    {
        if (!timeStop)
        {
            currentTime += Time.deltaTime;
    
            float minutes = currentTime/60;
            float seconds = currentTime % 60;
    
            TimeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (finalScoreDynamic)
        {
            // print("Dynamic Points");
            GainPoints(31);

            if (totalPoints >= FinalScore)
            {
                finalScoreDynamic = false;

                totalPoints = FinalScore;
                PointsDisplay.text = "Score :: " + totalPoints.ToString();

                Invoke("GoToNextLevel", 2);
                // print("Heres the Final Score");
            }
        }
    }

//=========================================================================

    public void EndReached()
    {
        timeStop = true;
    }

    public void StopTimeForCutscene(bool value)
    {
        timeStop = value;
    }

    public float GetTimeStamp()
    {
        return currentTime;
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }

    public int GetTotalAuras()
    {
        return aurasCollected;
    }

//================================================================================================
//================================================================================================
//================================================================================================

    public void DisplayTotalForLevel(int grandTotal)
    {
        FinalScore = grandTotal;
        finalScoreDynamic = true;
    }

    void GoToNextLevel()
    {
        screenScript.EndLevelFade();
        Invoke("TransitionToNextLevel", 1.15f);
    }

    public void ReloadCurrentLevel()
    {
        screenScript.EndLevelFade();
        Invoke("ReloadTheLevel", 1.15f);
    }

    void ReloadTheLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerQuittingGame()
    {
        screenScript.EndLevelFade();
        Invoke("Quitting", 1.15f);
    }

    void Quitting()
    {
        SceneManager.LoadScene(0);
    }

    void TransitionToNextLevel()
    {
        if (PlayerPrefs.GetInt("ManualLoading") < 1) // is zero
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else // is a one
            {
                SceneManager.LoadScene(0);
            }
    }

//================================================================================================
//================================================================================================
//================================================================================================

    public void playerCanBeCountered()
    {
        counterable = true;
    }

    public bool isCounterable()
    {
        return counterable;
    }

    public void revertCounter()
    {
        counterable = false;
    }

    public void GainPoints(int amount)
    {
        totalPoints += amount;

        PointsDisplay.text = "Score :: " + totalPoints.ToString();
    }

    public void PlayerDamaged(Vector2 sendOffDir)
    {
        if (!playerIFrame)
        {
            if (aurasCollected > 0)
            {
                DropAura();

                StartCoroutine(spriteFlash());

                PlayerMovementScript.enabled = false;
                PlayerMovementScript.isPlayerDamaged(true);

                // print(PlayerMovementScript.enabled);

                if (sendOffDir != Vector2.zero)
                {
                    transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    transform.GetComponent<Rigidbody2D>().AddForce(sendOffDir, ForceMode2D.Impulse);
                }
                
                nextLifeinc = 1;
                aurasCollected = 0;
            } else
                {
                    PlayerDeath();
                    return;
                }

            playerIFrame = true;
    
            Invoke("PlayerGetUp", 1.25f);
            Invoke("turnOffPlayerIFrame", 2.25f);
        }
    }

    void PlayerGetUp()
    {
        PlayerMovementScript.enabled = true;
        PlayerMovementScript.isPlayerDamaged(false);
    }

    void turnOffPlayerIFrame()
    {
        playerIFrame = false;
    }

    void DropAura()
    {
        if (aurasCollected >= 5)
        {
            // drop 5
            for (int i = 0; i < 5; i++)
            {
                GameObject droppedObject = Instantiate(AuraObject, transform.position, AuraObject.transform.rotation);
                droppedObject.transform.GetComponent<Rigidbody2D>().velocity = dropDirections[i];
            }
        } else if (aurasCollected >= 2 && aurasCollected <= 4)
            {
                // drop the aurasCollected amount
                GetRandomDirections();
            } else
                {
                    // drop just one aura
                    GameObject droppedObject = Instantiate(AuraObject, transform.position, AuraObject.transform.rotation);
                    droppedObject.transform.GetComponent<Rigidbody2D>().velocity = dropDirections[Random.Range(0, 4)];
                }

        UpdateAuraDisplay();
    }

    void UpdateAuraDisplay()
    {
        AuraDisplay.text = "Auras :: " + aurasCollected.ToString();
    }

    void UpdateTotalLives()
    {
        PlayerPrefs.SetInt("totalLives", totalLives);
        LifeDisplay.text = "x " + totalLives.ToString();
    }

    void CheckForLifeGift()
    {
        if (aurasCollected >= (100 * nextLifeinc))
        {
            nextLifeinc++;
            totalLives++;
            UpdateTotalLives();
        }
    }

    void PlayerDeath()
    {
        isDead = true;

        totalLives--;
        UpdateTotalLives();

        Destroy(PlayerMovementScript);

        playerAnim.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(playerAnim.transform.GetComponent<Rigidbody2D>());

        playerAnim.transform.GetComponent<Collider2D>().enabled = false;

        // death anim & screen fade in & reload
        playerAnim.Play("playerDeath");

        if (totalLives > 0)
        {
            Invoke("ReloadScene", 2);
        } else
            {
                // game over screen
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                GameOverScreenAnim.Play("gameover");
            }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnTriggerEnter2D(Collider2D collider2D) // Pick Up Aura
    {
        if (!isDead)
        {
            if (collider2D.transform.tag == "redAura" || collider2D.transform.tag == "blueAura")
            {
                if (collider2D.transform.tag == "blueAura")
                {
                    aurasCollected++;
                    UpdateAuraDisplay();
    
                    AuraCollectedSound.Play();
                    Destroy(collider2D.transform.gameObject);
                }
    
                if (collider2D.transform.tag == "redAura")
                {
                    aurasCollected += 3;
                    UpdateAuraDisplay();
    
                    AuraCollectedSound.Play();
                    Destroy(collider2D.transform.gameObject);
                }
                
                CheckForLifeGift();
            }
        }
    }

    IEnumerator spriteFlash()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(0.07f);

        spriteRenderer.color = fullColor;

        yield return new WaitForSeconds(0.07f);

        if (playerIFrame)
        {
            StartCoroutine(spriteFlash());
        }
    }

    void GetRandomDirections()
    {
        int dropAmount = aurasCollected;

        List<int> r_Dir = new List<int>();

        aurasCollected = 0;

        r_Dir.Add(Random.Range(0, 4)); // first direction

        for (int i = 1; i < dropAmount;)
        {
            int r_index = Random.Range(0, 4);

            if (!r_Dir.Contains(r_index))
            {
                r_Dir.Add(r_index);
                i++;
            }
        }

        int[] dir_array = r_Dir.ToArray();

        for (int i = 0; i < dropAmount; i++)
        {
            GameObject droppedObject = Instantiate(AuraObject, transform.position, AuraObject.transform.rotation);
            droppedObject.transform.GetComponent<Rigidbody2D>().velocity = dropDirections[dir_array[i]];
        }
    }
}//EndScript