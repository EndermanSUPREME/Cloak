using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auraPlacementScript : MonoBehaviour
{
    [SerializeField] float x, oX;
    [SerializeField] float y, oY;

    [SerializeField] int nearestWhole, nearestWhole2;
    [SerializeField] float nearestDecimal, nearestDecimal2;

    void OnDrawGizmosSelected()
    {
        oX = transform.position.x;
        oY = transform.position.y;

        nearestWhole = (int)oX;
        nearestWhole2 = (int)oY;

// ============================================================================

        if (oX < nearestWhole)
        {
            nearestDecimal = nearestWhole - 0.5f;
        } else if (oX > nearestWhole)
            {
                nearestDecimal = nearestWhole + 0.5f;
            } else
                {
                    nearestDecimal = nearestWhole;
                }

        if (oY < nearestWhole2)
        {
            nearestDecimal2 = nearestWhole2 - 0.5f;
        } else if (oY > nearestWhole2)
            {
                nearestDecimal2 = nearestWhole2 + 0.5f;
            } else
                {
                    nearestDecimal2 = nearestWhole2;
                }

// ============================================================================

        float d1 = Mathf.Abs(nearestDecimal - oX);
        float d2 = Mathf.Abs(nearestWhole - oX);

        if (d1 < d2)
        {
            x = nearestDecimal;
        } else
            {
                x = nearestWhole;
            }

        float d3 = Mathf.Abs(nearestDecimal2 - oY);
        float d4 = Mathf.Abs(nearestWhole2 - oY);

        if (d3 < d4)
        {
            y = nearestDecimal2;
        } else
            {
                y = nearestWhole2;
            }

        transform.position = new Vector3(x, y, 0);
    }
}//EndScript