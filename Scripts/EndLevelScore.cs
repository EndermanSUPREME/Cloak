using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelScore : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerStats playerStatScript;
    playerMovement playerMovementScript;

    Transform Player;
    [SerializeField] Transform MainCamera, CameraStopPoint;
    [SerializeField] float speed;
    bool moveCamera = false, movePlayer = false;

    void Start()
    {
        playerStatScript = GameObject.FindObjectOfType<PlayerStats>();
        playerMovementScript = GameObject.FindObjectOfType<playerMovement>();
        Player = GameObject.Find("PlayerSprite").transform;
    }

    void FixedUpdate()
    {
        if (moveCamera)
        {
            Vector3 sPoint = new Vector3(CameraStopPoint.position.x, CameraStopPoint.position.y, MainCamera.position.z);
            MainCamera.position = Vector3.MoveTowards(MainCamera.position, sPoint, speed * Time.deltaTime);

            if ((MainCamera.position == sPoint))
            {
                CalcEndScore();
                moveCamera = false;
            }
        }

        if (movePlayer)
        {
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(6, Player.GetComponent<Rigidbody2D>().velocity.y);
            Player.GetComponent<Animator>().SetFloat("speed", Player.GetComponent<Rigidbody2D>().velocity.x);
        }
    }

    void AutoMovePlayer()
    {
        playerMovementScript.enabled = false;

        playerStatScript.EndReached();

        movePlayer = true;
        moveCamera = true;

        Invoke("StopPlayerAutoMove", 2);
    }

    void StopPlayerAutoMove()
    {
        movePlayer = false;
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void CalcEndScore()
    {
//============================================================================

        float timeScore = playerStatScript.GetTimeStamp();
        int tPoints = playerStatScript.GetTotalPoints();
        int tAura = playerStatScript.GetTotalAuras();
        
        int grandScore = (int) Mathf.Round( ( ( ( 300 / timeScore) + 0.65f) * tAura) * 8) + tPoints;
        playerStatScript.DisplayTotalForLevel(grandScore);
        // print(grandScore);
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform == Player)
            AutoMovePlayer();
    }
}//EndScript