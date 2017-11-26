using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectOnOff : MonoBehaviour
{

    public GameObject gameObject;
    private bool defaultState;
    private bool on;

    // Use this for initialization
    void Start()
    {

        defaultState = false;
        gameObject.SetActive(defaultState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || GvrControllerInput.AppButtonDown)
        {
            on = !on;
            gameObject.SetActive(on);
        }
    }
}
