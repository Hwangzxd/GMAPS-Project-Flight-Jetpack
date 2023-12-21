using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }
    }
}
