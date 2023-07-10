using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centipedeBossScript : MonoBehaviour
{
    [SerializeField] GameObject CentipedeObject, Blockage, centipedeHead, DeathParticleSys, funnelCollider;
    [SerializeField] Collider2D triggerCollider;
    Transform playerObject;

    void Start()
    {
        playerObject = GameObject.Find("PlayerSprite").transform;

        CentipedeObject.SetActive(false);
        Blockage.SetActive(false);
        funnelCollider.SetActive(false);
    }

    void Update()
    {
        if (CentipedeObject == null || !centipedeHead.activeSelf)
        {
            Blockage.SetActive(false);
            CentipedeObject.SetActive(false);
            CentipedAlt_Death();
            funnelCollider.SetActive(true);
            this.enabled = false;
        }
    }

    void CentipedAlt_Death()
    {
        AudioSource DeathSound = GameObject.Find("BossDeathSound").transform.GetComponent<AudioSource>();

        DeathSound.Play();

        if (DeathParticleSys != null)
        {
            GameObject particle = Instantiate(DeathParticleSys, centipedeHead.transform.position, DeathParticleSys.transform.rotation);
            Destroy(particle, 2.5f);
        }
    }

    void ReleaseTheCentipede()
    {
        CentipedeObject.SetActive(true);
        Blockage.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform == playerObject)
        {
            ReleaseTheCentipede();
            triggerCollider.enabled = false;
        }
    }
}//EndScript