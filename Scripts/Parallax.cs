using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length, startPos;
    [SerializeField] Transform cam;
    [SerializeField] float ParallaxEffect = 0;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (cam.position.x * (1 - ParallaxEffect));
        float dist = (cam.position.x * ParallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += length;
        } else if (temp < startPos - length)
            {
                startPos -= length;
            }
    }
}//EndScript