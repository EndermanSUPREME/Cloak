using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonFireBallScript : MonoBehaviour
{
    [SerializeField] GameObject impact;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -3.25f);
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        Instantiate(impact, collision2D.contacts[0].point, impact.transform.rotation);

        if (collision2D.transform.GetComponent<PlayerStats>() != null)
        {
            collision2D.transform.GetComponent<PlayerStats>().PlayerDamaged(Vector2.zero);
            collision2D.transform.position += new Vector3(0, 0.5f, 0);
        }

        Destroy(gameObject);
    }
}//EndScript