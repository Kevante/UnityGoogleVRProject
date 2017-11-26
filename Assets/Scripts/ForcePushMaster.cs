using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to GameObject with ForcePush Trigger
public class ForcePushMaster : MonoBehaviour {

    public ParticleSystem forcePushParticleSystem;
    public AudioSource audioSource;
    public float thrust = 700;
    
    private Rigidbody targetRigidBody;
    private Vector2 touchPosition;

    bool isTouchPositionUp() { return (touchPosition.y < .4 && (touchPosition.x > .4 && touchPosition.x < .6)); }
    bool isForcePushCalled() { return GvrControllerInput.ClickButtonDown && isTouchPositionUp(); }

    // Getters
    Rigidbody getTargetRigidBody(Collider objectInTriggerZone) { return objectInTriggerZone.GetComponent<Rigidbody>(); }
    Vector3 getDirectionOfForcePush() { return Camera.main.transform.forward + Camera.main.transform.up * .3f; }

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
        targetRigidBody.AddForce(getDirectionOfForcePush() * thrust, ForceMode.Force);
    }

    // Drivers
    private void Start() {
        forcePushParticleSystem.Stop();
        audioSourceDefaults();
    }

    private void Update() {
        setTouchPosition();

        if (isForcePushCalled()) {
            forcePushParticleSystem.Play();
            audioSource.Play();
        }
    }

    private void OnTriggerStay(Collider objectInTriggerZone) {
        targetRigidBody = getTargetRigidBody(objectInTriggerZone);
        setTouchPosition();

        if (isForcePushCalled()) {
            applyForce();
        }
    }
}
