using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour {

    private AudioSource audioSource;
    private bool defaultAudio;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        defaultAudio = false;

        audioSource.enabled = defaultAudio;
    }

    // Enable Audio Source
    public void TogglePlay()
    {
            audioSource.enabled = !audioSource.enabled;
    }
}
