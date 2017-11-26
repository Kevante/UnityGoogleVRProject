using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberSparkSound : MonoBehaviour {

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        audioSource.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        audioSource.enabled = false;
    }
}
