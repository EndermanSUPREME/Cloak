using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class distantDragonComponent : MonoBehaviour
{
    [SerializeField] DragonBossScript dragonBossCore;
    int timerForAttack, numberOfAttacks = 0;

    void Start()
    {
        dragonBossCore.distantDragon.Play("distantAppear");
        SetUpNextAttack();
    }

    void SetUpNextAttack()
    {
        if (numberOfAttacks < 4)
        {
            numberOfAttacks++;
            timerForAttack = Random.Range(6, 10);
            StartCoroutine(RunAttackAnim(timerForAttack));
        } else
            {
                ComeUpClose();
                numberOfAttacks = 0;
            }
    }

    IEnumerator RunAttackAnim(int t)
    {
        yield return new WaitForSeconds(t);
        dragonBossCore.distantDragon.Play("distantAttack");
        SetUpNextAttack();
    }

    void ComeUpClose()
    {
        dragonBossCore.ComeClose();
    }

    void AttackEvent()
    {
        dragonBossCore.DragonDistantAttack();
    }

    public void RestartDistantDragon()
    {
        dragonBossCore.distantDragon.Play("distantAppear");
        SetUpNextAttack();
    }

}//EndScript