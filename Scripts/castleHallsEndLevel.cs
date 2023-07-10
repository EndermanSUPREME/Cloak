using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class castleHallsEndLevel : MonoBehaviour
{
    public EndLevelScore endScoreScript;

    [SerializeField] GameObject dialogueObject, blackScreen;

    Transform Player;
    [SerializeField] ParticleSystem KeldorParticles;
    [SerializeField] PlayerStats playerStatsScript;
    [SerializeField] Text dialogueDisplay;
    protected string threat = "Keldor : So You've gotten this far for your little girlfriend? Let's see you get out of this!\n[LeftShift To Continue]";
    [SerializeField] bool endTriggered = false;
    MenuScript screenScript;

    void Start()
    {
        screenScript = GameObject.FindObjectOfType<MenuScript>();
        Player = GameObject.Find("PlayerSprite").transform;

        GetComponent<Collider2D>().enabled = true;

        dialogueObject.SetActive(false);
        blackScreen.SetActive(false);
    }

    void Update()
    {
        // print(dialogueDisplay.text = " :: " + threat + " == " + (dialogueDisplay.text.Equals(threat)));

        if ((dialogueDisplay.text.Equals(threat)) && Input.GetKeyDown(KeyCode. LeftShift) && !endTriggered)
        {
            endTriggered = true;
            EndOfDialogue();
        }
    }

    IEnumerator LoadText(string str)
    {
        string currentSentence = str;

        if (!dialogueDisplay.text.Equals(currentSentence))
        {
            foreach (char letter in currentSentence.ToCharArray())
            {
                dialogueDisplay.text += letter;
                yield return new WaitForSeconds(0.035f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform == Player)
        {
            GetComponent<Collider2D>().enabled = false;

            SmallDialogue();
        }
    }

    void SmallDialogue()
    {
        Player.GetComponent<playerMovement>().isPlayerDamaged(true);
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        playerStatsScript.StopTimeForCutscene(true);

        Player.GetComponent<Animator>().SetFloat("speed", 0);
        Player.GetComponent<Animator>().Play("idleRight");

        dialogueObject.SetActive(true);

        StartCoroutine(LoadText(threat));
    }

    public void EndOfDialogue()
    {
        dialogueObject.SetActive(false);

        KeldorParticles.Play();
        Invoke("BlackOutScreen", 3.20f);
    }

    void BlackOutScreen()
    {
        blackScreen.SetActive(true);
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