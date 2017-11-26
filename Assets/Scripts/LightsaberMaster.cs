using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberMaster : MonoBehaviour {

    public Transform startPosition;
    public Transform endPosition;
    public LineRenderer centerBeam;
    public LineRenderer outerBeam;
    public AudioSource SaberOnAudio;
    public AudioSource SaberOffAudio;
    public AudioSource SaberHumAudio;
    public Light beamGlow;
    public float beamGlowIntesity = .4f;

    private Vector3 extendedPosition;
    private float textureOffset;
    private bool isOn = false;

    // Methods
    void extendLightsaberBeam() {
        endPosition.localPosition = Vector3.Lerp(endPosition.localPosition, extendedPosition, Time.deltaTime * 5f);
    }
    void retractLightsaberBeam() {
        endPosition.localPosition = Vector3.Lerp(endPosition.localPosition, startPosition.localPosition, Time.deltaTime * 5f);
    }
    void updateBeamPositions() {
        centerBeam.SetPosition(0, startPosition.position);
        centerBeam.SetPosition(1, endPosition.position);
        outerBeam.SetPosition(0, startPosition.position);
        outerBeam.SetPosition(1, endPosition.position);
    }
    void panTexture() {
        textureOffset -= Time.deltaTime * 2f;
        if (textureOffset < -10) {
            textureOffset += 10f;
        }
        centerBeam.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset, 0f));
        outerBeam.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset, 0f));
    }
    void disableBeamsIfRetracted() {
        if (Vector3.Distance(endPosition.localPosition, startPosition.localPosition) <.1) {     // Beam objects toggle off when their position is close to start position
            centerBeam.enabled = false;
            outerBeam.enabled = false;
        } else {
            centerBeam.enabled = true;
            outerBeam.enabled = true;
        }
    }
    void setBeamGlowIntensity() {
        beamGlow.intensity = Vector3.Distance(endPosition.localPosition, startPosition.localPosition) * beamGlowIntesity;   // intensity proportional to beam length
    }
    void saberHumDefaults() {
        SaberHumAudio.loop = true;
        SaberHumAudio.playOnAwake = true;
        SaberHumAudio.enabled = false;
    }
    void toggleSaberOnOff() {
        SaberHumAudio.enabled = false;
        if (isOn) {
            isOn = false;     // Turn off
            SaberOffAudio.Play();
        } else {
            isOn = true;      // Turn On
            SaberOnAudio.Play();
            SaberHumAudio.enabled = true;
        }
    }

    // Use this for initialization
    void Start() {
        extendedPosition = endPosition.localPosition;
        endPosition.localPosition = startPosition.localPosition;
        saberHumDefaults();
    }

    // Update is called once per frame
    void Update() {
        disableBeamsIfRetracted();
        setBeamGlowIntensity();
        // turn lightsaber on and off
        if (GvrControllerInput.AppButtonDown) {
            toggleSaberOnOff();
        }
        // extend the line
        if (isOn) {
            extendLightsaberBeam();
        } else {
            retractLightsaberBeam();
        }
        updateBeamPositions();  // update line positions
        panTexture();
    }
}
