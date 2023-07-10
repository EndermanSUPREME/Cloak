using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeSegmentScript : MonoBehaviour
{
    public float segmentLength, speed;
    [SerializeField] Transform[] segments, destPoints;
    Transform tarDest;
    int destIndex = 1;

    void Start()
    {
        tarDest = destPoints[destIndex];
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, tarDest.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, tarDest.position) < 0.05f)
        {
            if (destIndex < destPoints.Length - 1)
            {
                destIndex++;
            } else
                {
                    destIndex = 0;
                }

            tarDest = destPoints[destIndex];
        }

        int i2 = 0;

        for (int i = 0; i < segments.Length; i++)
        {
            if (!segments[i].gameObject.activeSelf)
            {
                i2++;

                if (i2 > 7)
                {
                    DestroyAllSegments();
                }
            }
        }
    }

    void DestroyAllSegments()
    {
        foreach (Transform seg in segments)
        {
            Destroy(seg.gameObject);
        }

        transform.GetComponent<HealthScript>().dead();
    }

    void FixedUpdate()
    {
        RelocateSegments();
    }

    void RelocateSegments()
    {
        Vector3[] nPos = new Vector3[10];
        int i2 = 0;

        for (int i = 1; i < 11; i++)
        {
            // Vector3 genDir = (segments[1].position - segments[0].position).normalized;
            // segments[1].position = segments[0].position + genDir * segmentLength;

            Vector3 gDir = (segments[i].position - segments[i-1].position).normalized;
            nPos[i2] = segments[i-1].position + gDir * segmentLength;
            i2++;
        }

        OutputNewPositions(nPos);
    }

    void OutputNewPositions(Vector3[] nPos)
    {
        for (int i = 1; i < 11; i++)
        {
            segments[i].position = nPos[i-1];
        }
    }

}//EndScript