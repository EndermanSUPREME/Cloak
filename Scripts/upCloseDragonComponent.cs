using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upCloseDragonComponent : MonoBehaviour
{
    [SerializeField] DragonBossScript dragonBossCore;
    [SerializeField] HealthScript healthScript;

    [SerializeField] Collider2D mainHitBox, blockerBox;

    int t = 12;
    bool playHurtAnim = false;

    public void SitClose() // animation event
    {
        StartCoroutine(ScheduleSwap());
    }

    public void HitBoxesDisabled()
    {
        mainHitBox.enabled = false;
        blockerBox.enabled = false;
    }

    void Update()
    {
        if (dragonBossCore.closeUpDragon.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            if (healthScript.hasBeenInjured)
            {
                if (!playHurtAnim)
                {
                    Hurt();
                    playHurtAnim = true;
                }
            } else
                {
                    playHurtAnim = false;
                    
                    mainHitBox.enabled = true;
                    blockerBox.enabled = true;
                }
        }
    }

    void Hurt()
    {
        dragonBossCore.closeUpDragon.Play("injured");
        mainHitBox.enabled = false;
        blockerBox.enabled = false;
    }

    IEnumerator ScheduleSwap()
    {
        // print("Coroutine Active");
        mainHitBox.enabled = true;
        blockerBox.enabled = true;

        yield return new WaitForSeconds(t);
        
        mainHitBox.enabled = false;
        blockerBox.enabled = false;
        GoDistant();
    }

    void GoDistant()
    {
        // print("Swapping");
        dragonBossCore.GoDistant();
    }

}//EndScript