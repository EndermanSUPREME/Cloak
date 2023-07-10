using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelMusicHandler : MonoBehaviour
{
    [SerializeField] AudioClip[] musicTracks;

    void Awake()
    {
        GetComponent<AudioSource>().clip = musicTracks[UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex];
    }

    void Start()
    {
        GetComponent<AudioSource>().Play();
    }
}//EndScript