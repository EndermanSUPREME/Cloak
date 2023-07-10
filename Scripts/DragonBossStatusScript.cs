using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossStatusScript : MonoBehaviour
{
    [SerializeField] GameObject RightSideBossBorder, CloseUpDragonObject;

    void Update()
    {
        if (CloseUpDragonObject == null)
        {
            RightSideBossBorder.SetActive(false);
        }
    }
}//EndScript