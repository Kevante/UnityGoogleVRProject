using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodRacer : MonoBehaviour {

    public GameObject podracer;
    public GameObject leftThrottleLever;
    public Transform leftThrottleMinPosition;
    public Transform leftThrottleMaxPosition;
    public GameObject rightThrottleLever;
    public Transform rightThrottleMinPosition;
    public Transform rightThrottleMaxPosition;
    public float thrustForce = 60;
    public float hoverForce = 65f;
    public float hoverHeight = 3f;
    public float turnSpeed = 5.0f;
    public float tiltAngle = 60.0f;
    public float stability = 0.3f;
    public float StabilitySpeed = 7.0f;

    private Rigidbody podracerRigidBody;
    private Vector3 leftThrottleCurrentPosition;
    private Vector3 rightThrottleCurrentOPosition;

    private Quaternion podRotation;
    private float podLeftTurnYAxis;
    private float podLeftTurnZAxis;
    private float podRightTurnYAxis;
    private float podRightTurnZAxis;

    // Getters
    float getLeftLeverPosition() { return Vector3.Distance(leftThrottleCurrentPosition, leftThrottleMinPosition.localPosition); }
    float getRightLeverPosition() { return Vector3.Distance(rightThrottleCurrentOPosition, rightThrottleMinPosition.localPosition); }

    // Setters

    // Methods
    void moveLeftLeverUp() {
        leftThrottleLever.transform.localPosition = Vector3.MoveTowards(leftThrottleLever.transform.localPosition, leftThrottleMaxPosition.localPosition, .05f);
    }
    void moveLeftLeverDown() {
        leftThrottleLever.transform.localPosition = Vector3.MoveTowards(leftThrottleLever.transform.localPosition, leftThrottleMinPosition.localPosition, .05f);
    }
    void moveRightLeverUp() {
        rightThrottleLever.transform.localPosition = Vector3.MoveTowards(rightThrottleLever.transform.localPosition, rightThrottleMaxPosition.localPosition, .05f);
    }
    void moveRightLeverDown() {
        rightThrottleLever.transform.localPosition = Vector3.MoveTowards(rightThrottleLever.transform.localPosition, rightThrottleMinPosition.localPosition, .05f);
    }
    void leftThrust() {
        podracerRigidBody.AddForce(podracer.transform.forward * getLeftLeverPosition() * thrustForce, ForceMode.Force);
    }
    void rightThrust() {
        podracerRigidBody.AddForce(podracer.transform.forward * getRightLeverPosition() * thrustForce, ForceMode.Force);
    }
    void hoverPodRacer() {
        podracerRigidBody.AddForce(podracer.transform.forward * hoverForce, ForceMode.Force);
    }

    // Use this for initialization
    void Start() {
        podracerRigidBody = podracer.GetComponent<Rigidbody>();
        leftThrottleCurrentPosition = leftThrottleLever.transform.localPosition;
        rightThrottleCurrentOPosition = rightThrottleLever.transform.localPosition;
        leftThrottleLever.transform.localPosition = leftThrottleMinPosition.localPosition;
        rightThrottleLever.transform.localPosition = rightThrottleMinPosition.localPosition;

        podRotation = podracer.transform.rotation;
        podLeftTurnYAxis = podRotation.y;
        podLeftTurnZAxis = podRotation.z;
        podRightTurnYAxis = podRotation.y;
        podRightTurnZAxis = podRotation.z;
    }
    
    void FixedUpdate() {
        // Update lever positions
        leftThrottleCurrentPosition = leftThrottleLever.transform.localPosition;
        rightThrottleCurrentOPosition = rightThrottleLever.transform.localPosition;

        // keyboard input to move lever positions
        if (Input.GetKey("q")) {
            moveLeftLeverUp();
            Debug.Log("q");
        }
        if (Input.GetKey("a")) {
            moveLeftLeverDown();
            Debug.Log("a");
        }
        if (Input.GetKey("w")) {
            moveRightLeverUp();
        }
        if (Input.GetKey("s")) {
            moveRightLeverDown();
        }

        // Forward thrust
        leftThrust();
        rightThrust();
        
        // Turn
        podLeftTurnYAxis = getRightLeverPosition() * turnSpeed * Time.deltaTime;
        podRightTurnYAxis = getLeftLeverPosition() * turnSpeed * Time.deltaTime;
        podracerRigidBody.AddTorque(podracer.transform.up * -podLeftTurnYAxis, ForceMode.VelocityChange);
        podracerRigidBody.AddTorque(podracer.transform.up * podRightTurnYAxis, ForceMode.VelocityChange);

        // Tilt
        podLeftTurnZAxis = getRightLeverPosition() * tiltAngle * .5f;
        podRightTurnZAxis = getLeftLeverPosition() * tiltAngle * .5f;
        podracerRigidBody.AddTorque(podracer.transform.forward * podLeftTurnZAxis, ForceMode.Acceleration);
        podracerRigidBody.AddTorque(podracer.transform.forward * -podRightTurnZAxis, ForceMode.Acceleration);

        // hover
        Ray ray = new Ray(podracer.transform.position, -podracer.transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, hoverHeight)) {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            podracerRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        // Stabalaized hover
        Vector3 predictedUp = Quaternion.AngleAxis(
            podracerRigidBody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / StabilitySpeed,
            podracerRigidBody.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        podracerRigidBody.AddTorque(torqueVector * StabilitySpeed * StabilitySpeed);
    }
}
