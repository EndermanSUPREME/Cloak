using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finalEndTrigger : MonoBehaviour
{
    Transform Player;
    [SerializeField] PlayerStats playerStatsScript;
    public EndLevelScore endScoreScript;
    Animator anim;
    public Animator heartAnim;
    MenuScript screenScript;

    void Start()
    {
        screenScript = GameObject.FindObjectOfType<MenuScript>();
        anim = GetComponent<Animator>();
        Player = GameObject.Find("PlayerSprite").transform;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform == Player)
        {
            GetComponent<Collider2D>().enabled = false;

            FreezePlayer();
        }
    }

    void FreezePlayer()
    {
        Player.GetComponent<playerMovement>().isPlayerDamaged(true);
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        playerStatsScript.StopTimeForCutscene(true);

        Player.GetComponent<Animator>().SetFloat("speed", 0);
        Player.GetComponent<Animator>().Play("idleLeft");

        Invoke("FinalAnimation", 1.5f);
    }

    void FinalAnimation()
    {
        anim.Play("Epilogue");
    }

    public void heartDisplay()
    {
        heartAnim.Play("heartAnim");
        
        Invoke("GetFinalPoints", 2);
    }

    void GetFinalPoints()
    {
        endScoreScript.CalcEndScore();
        Invoke("LoadNextScene", 3);
    }

    void LoadNextScene()
    {
        screenScript.EndLevelFade();
        Invoke("TransitionToNextLevel", 1.15f);
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

}//EndScript