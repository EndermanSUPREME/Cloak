using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class finalBossScript : MonoBehaviour
{
    public GameObject leftBorder, rightBorder, Keldor, dialogueObject, DragonBoss;

    Transform Player;
    [SerializeField] ParticleSystem KeldorParticles;
    [SerializeField] PlayerStats playerStatsScript;
    [SerializeField] Text dialogueDisplay;
    protected string threat = "Keldor : You? How did you get out?! No Matter... This time I'll finish you off, with my Final Form!\n[LeftShift To Continue]";
    [SerializeField] bool endTriggered = false;

    void Start()
    {
        DragonBoss.SetActive(false);

        leftBorder.SetActive(false);
        rightBorder.SetActive(false);

        Player = GameObject.Find("PlayerSprite").transform;

        GetComponent<Collider2D>().enabled = true;

        dialogueObject.SetActive(false);
    }

    void Update()
    {
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

            leftBorder.SetActive(true);
            rightBorder.SetActive(true);
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
        Invoke("swapKeldor", 2f);
    }

    void swapKeldor()
    {
        Keldor.SetActive(false);
        Invoke("SpawnTheDragon", 6.5f);
    }

    void SpawnTheDragon()
    {
        DragonBoss.SetActive(true);

        playerStatsScript.StopTimeForCutscene(false);
        Player.GetComponent<playerMovement>().isPlayerDamaged(false);
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

}//EndScript