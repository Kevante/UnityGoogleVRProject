using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light on;
    private bool defaultLight;

    // Use this for initialization
    void Start()
    {
        on = GetComponent<Light>();
        defaultLight = false;

        on.enabled = defaultLight;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || GvrControllerInput.AppButtonDown)
        {
            on.enabled = !on.enabled;
        }
    }
}
