using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGrabMaster : MonoBehaviour {

    public AudioSource audioSource;
    public float thrust = 550;
    public float range = 10;
    public float liftSpeed = 4.0F;
    public float liftDistance = 6.0f;

    private GameObject grabbedObject;
    private Ray cameraRay;
    private RaycastHit hit;
    private Vector3 currentPosition;
    private Vector3 endPosition;
    private Vector2 touchPosition;
    private bool canGrab;
    private bool isGrabbed;
    private bool isOn = true;
    private float startTime;
    private float journeyLength;

    bool isTouchPositionCenter() { return ((touchPosition.y > .4 && touchPosition.y < .6) && (touchPosition.x > .4 && touchPosition.x < .6)); }
    bool isTouchPositionLeft() { return ((touchPosition.y > .4 && touchPosition.y < .6) && touchPosition.x < .4); }
    bool isTouchPositionRight() { return ((touchPosition.y > .4 && touchPosition.y < .6) && touchPosition.x > .6); }
    bool isForceGrabCalled() { return GvrControllerInput.ClickButtonDown && isTouchPositionCenter(); }
    bool didRaycastHitObject() { return Physics.Raycast(cameraRay, out hit, range); }

    // Setters
    void setAudioSourceState(bool isOn) {
        audioSource.enabled = isOn;
    }
    void setTouchPosition() {
        touchPosition = GvrControllerInput.TouchPos;
    }
    void setCameraRay() {
        cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward * range);
    }
    void setCurrentPosition() {
        currentPosition = grabbedObject.transform.position;
    }
    void setStartTime() {
        startTime = Time.time;
    }
    void setGrabbedObject() {
        grabbedObject = hit.collider.gameObject;
    }
    void setEndPosition() {
        endPosition = currentPosition + Vector3.up * liftDistance * .1f;
    }
    void setJourneyLength() {
        journeyLength = Vector3.Distance(currentPosition, endPosition);
    }

    void audioSourceDefaults() {
        audioSource.playOnAwake = true;
        audioSource.loop = true;
    }

    // Methods
    void startForceGrab() {
        isGrabbed = true;
        setGrabbedObject();
        setCurrentPosition();
        setEndPosition();
        setJourneyLength();
        setStartTime();
    }
    void endForceGrab() {
        canGrab = false;
        grabbedObject = null;
        setAudioSourceState(!isOn);
        audioSourceDefaults();
    }
    void forceLift() {
        float distanceCovered = (Time.time - startTime) * liftSpeed;
        float fractionOfJourney = distanceCovered / journeyLength;
        grabbedObject.transform.position = Vector3.Lerp(currentPosition, endPosition, fractionOfJourney);
        grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        setAudioSourceState(isOn);
    }
    void forceThrowLeft() {
        grabbedObject.GetComponent<Rigidbody>().AddForce(-Camera.main.transform.right * thrust, ForceMode.Force);
    }
    void forceThrowRight() {
        grabbedObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.right * thrust, ForceMode.Force);
    }

    // Drivers
    // Initializes
    private void Start() {
        setAudioSourceState(!isOn);
        setStartTime();
    }
    // Updates every frame
    private void Update() {
        setCameraRay();
        setTouchPosition();
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);   // show ray in scene view for Debug purposes

        // shoot ray from camera
        // checks if object the ray hits has a rigidbody
        if (didRaycastHitObject()) {
            if (hit.rigidbody != null) {
                canGrab = true;
            }
        }

        // if center touchpad press, checks if target object was valid. 
        // If valid, begins to lift the object
        if (isForceGrabCalled()) {
            if (canGrab) {
                startForceGrab();
            }
        }

        // once touchpad is unpressed, object is no longer lifted
        if (GvrControllerInput.ClickButtonUp) {
            isGrabbed = false;
        }

        // object is lifted from current position to end position at speed setting
        if (isGrabbed) {
            forceLift();
        }

        if (isTouchPositionLeft() && isGrabbed) {
            forceThrowLeft();
            isGrabbed = false;
        }

        if (isTouchPositionRight() && isGrabbed) {
            forceThrowRight();
            isGrabbed = false;
        }

        if (!isGrabbed) {
            endForceGrab();
        }
    }
}
