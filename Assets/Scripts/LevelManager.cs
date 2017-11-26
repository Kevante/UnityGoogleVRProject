using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public string TargetScene;

    private float timeSinceLastClick = 0f;

    public void LoadLevel(string name) {
        Application.LoadLevel(name);
    }

    public void QuitRequest()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (GvrControllerInput.AppButton)
        {
            timeSinceLastClick += Time.deltaTime;

            if (timeSinceLastClick > 4f)
            {
                Application.LoadLevel(TargetScene);
            }
        }
        if (GvrControllerInput.AppButtonUp)
        {
            timeSinceLastClick = 0f;
        }
    }
}
