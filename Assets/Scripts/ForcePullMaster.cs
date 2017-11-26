using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to GameObject with ForcePull Trigger Collider
public class ForcePullMaster : MonoBehaviour {

    public ParticleSystem forcePullParticleSystem;
    public AudioSource audioSource;
    public float thrust = 550;

    private Rigidbody targetRigidBody;
    private Vector2 touchPosition;

    bool isTouchPositionDown() { return (touchPosition.y > .6 && (touchPosition.x > .4 && touchPosition.x < .6)); }
    bool isForcePullCalled() { return GvrControllerInput.ClickButtonDown && isTouchPositionDown(); }

    // Getters
    Rigidbody getTargetRigidBody(Collider objectInTriggerZone) { return objectInTriggerZone.GetComponent<Rigidbody>(); }
    Vector3 getDirectionOfForcePull() { return Camera.main.transform.forward - Camera.main.transform.up * .3f; }

    // Setters
    void setTouchPosition() {
        touchPosition = GvrControllerInput.TouchPos;
    }
    void audioSourceDefaults() {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // Methods
    void applyForce() {
        targetRigidBody.AddForce(-getDirectionOfForcePull() * thrust, ForceMode.Force);
    }

    // Drivers
    private void Start() {
        forcePullParticleSystem.Stop();
        audioSourceDefaults();
    }

    private void Update() {
        setTouchPosition();

        if (isForcePullCalled()) {
            forcePullParticleSystem.Play();
            audioSource.Play();
        }
    }

    private void OnTriggerStay(Collider objectInTriggerZone) {
        targetRigidBody = getTargetRigidBody(objectInTriggerZone);
        setTouchPosition();

        if (isForcePullCalled()) {
            applyForce();
        }
    }
}
