using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droppedAuraScript : MonoBehaviour
{
    public Collider2D triggerCollider, physicsCollider;

    void Start()
    {
        triggerCollider.enabled = false;
        Invoke("EnableCollider", 1.5f);
    }

    void EnableCollider()
    {
        Destroy(GetComponent<Rigidbody2D>());
        physicsCollider.enabled = false;
        
        triggerCollider.enabled = true;
        triggerCollider.isTrigger = true;
    }
}//EndScript