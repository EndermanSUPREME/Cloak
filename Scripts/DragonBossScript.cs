using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossScript : MonoBehaviour
{
    public Animator distantDragon, closeUpDragon;
    [SerializeField] distantDragonComponent distantDragonComp;
    public GameObject DistantObj, CloseUpObj, FireBallPref;
    public ParticleSystem FireBreath;
    [SerializeField] Transform[] fireRainSpawnPoints;

    public void ComeClose()
    {
        StartCoroutine(ComingCloser());
    }

    public void GoDistant()
    {
        StartCoroutine(GoFar());
    }

    IEnumerator ComingCloser()
    {
        distantDragon.Play("distantDisappear");
        yield return new WaitForSeconds(0.85f);
        closeUpDragon.Play("appearCloseUp");
    }

    IEnumerator GoFar()
    {
        closeUpDragon.Play("disappearCloseUp");
        yield return new WaitForSeconds(0.85f);
        distantDragonComp.RestartDistantDragon();
    }

    public void DragonDistantAttack() // anim event
    {
        FireBreath.Clear();
        FireBreath.Play();

        Invoke("SpawnFireRain", 1);
    }

    void SpawnFireRain()
    {
        FireBreath.Stop();

        for (int i = 0; i < fireRainSpawnPoints.Length; i++)
        {
            Instantiate(FireBallPref, fireRainSpawnPoints[i].position, FireBallPref.transform.rotation);
        }
    }
}//EndScript