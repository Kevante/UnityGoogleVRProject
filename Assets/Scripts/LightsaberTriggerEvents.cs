using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberTriggerEvents : MonoBehaviour {

    public void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("enemy"))
            collider.GetComponent<RebelTrooper>().kill();
    }

    void OnTriggerStay(Collider collider) {
        
    }

    void OnTriggerExit(Collider collider) {
        
    }
}
