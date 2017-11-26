using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public AudioSource shot;

	// Use this for initialization
	void Start () {
        shot.Play();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }
}
