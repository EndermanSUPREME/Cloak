using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class firstBossEvent : MonoBehaviour
{
    [SerializeField] GameObject LeftBorder, RightBorder, dialogueObject, bossObject, KeldorObject;
    Transform Player;
    [SerializeField] ParticleSystem KeldorParticles;
    [SerializeField] golemBossScript bossScript;
    [SerializeField] PlayerStats playerStatsScript;
    [SerializeField] Text dialogueDisplay;
    protected string threat = "Keldor : You must really want your girlfriend back don't you? How about I offer this; Lets see how you fare against my golem!\n[LeftShift To Continue]";
    [SerializeField] bool endTriggered = false;

    void Start()
    {
        Player = GameObject.Find("PlayerSprite").transform;

        LeftBorder.SetActive(false); RightBorder.SetActive(false);

        GetComponent<Collider2D>().enabled = true;

        KeldorObject.SetActive(true);
        bossObject.SetActive(false);

        dialogueObject.SetActive(false);
    }

    void Update()
    {
        // print(dialogueDisplay.text = " :: " + threat + " == " + (dialogueDisplay.text.Equals(threat)));

        if ((dialogueDisplay.text.Equals(threat)) && Input.GetKeyDown(KeyCode. LeftShift) && !endTriggered)
        {
            endTriggered = true;
            EndOfDialogue();
        }

        if (bossScript == null)
        {
            LeftBorder.SetActive(true); RightBorder.SetActive(false);
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
            LeftBorder.SetActive(true); RightBorder.SetActive(true);

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
        Invoke("ReplaceKeldorWithBoss", 2.25f);
        Invoke("LaunchFight", 7.25f);
    }

    public void ReplaceKeldorWithBoss()
    {
        KeldorObject.SetActive(false);
        bossObject.SetActive(true);
    }

    public void LaunchFight()
    {
        Player.GetComponent<playerMovement>().isPlayerDamaged(false);
        bossScript.allowedToMove();
        playerStatsScript.StopTimeForCutscene(false);
    }
}//EndScript