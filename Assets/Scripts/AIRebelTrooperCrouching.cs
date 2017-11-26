﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRebelTrooperCrouching : MonoBehaviour {

    public Transform target;
    public Transform head;
    public Transform gun;
    public Transform laserEmitter;
    public GameObject laser;
    public GameObject lightsaberParent;
    public AudioSource openFireOrder;
    public float baseSpeed = .04f;
    public float dangerZone = 3;
    public float backUpDistance = 2;
    public float laserSpeed = 100;
    public float accuracy = 1.5f;

    Animator characterAnimator;
    LightsaberMaster lightsaber;
    float timeDelay;
    bool isAiming = false;
    bool isShooting = false;
    bool isBackingUp = false;
    bool isEngaged = true;

    NavMeshAgent nav;

    bool isLightsaberOn() { return lightsaberParent; }
    bool isTargetInsideDangerZone() { return getDistanceToTarget() < dangerZone; }

    // Getters
    Vector3 getDirectionOfTarget() { return target.position - this.transform.position; }    // will move to face target in all directions
    Vector3 getDirectionOfTargetRandom() { return target.position - this.transform.position + new Vector3(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)); }
    Vector3 getDirectionOfTargetWithoutY() {                    // will move to face target, but remain on ground. Sets Y to Zero
        Vector3 directionOfTarget = getDirectionOfTarget();
        directionOfTarget.y = 0;
        return directionOfTarget;
    }
    float getDistanceToTarget() { return Vector3.Distance(target.position, this.transform.position); }

    // Methods
    void aim() {
        if (isLightsaberOn()) {
            characterAnimator.SetBool("isCrouchAiming", true);
        }
    }
    void openFire() {
        openFireOrder.enabled = true; ;
        if (!openFireOrder.isPlaying) {
            characterAnimator.SetBool("isCrouchFiring", true);
        }
    }
    void faceTarget() {
        head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(getDirectionOfTarget()), 0.2f);
    }
    void turnTowardsTarget() {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(getDirectionOfTargetWithoutY()), 0.2f);
    }
    void pointGunAtTarget() {
        gun.rotation = Quaternion.Slerp(gun.rotation, Quaternion.LookRotation(getDirectionOfTargetRandom()), 0.1f);
    }
    void fireLazer() {
        GameObject tempLaser = Instantiate(laser, laserEmitter.position, laserEmitter.rotation) as GameObject;
        Rigidbody tempLaserRigidBody = tempLaser.GetComponent<Rigidbody>();
        tempLaserRigidBody.AddForce(laserEmitter.transform.forward * laserSpeed);
        tempLaser.transform.Rotate(Vector3.left * 90);
        
        Destroy(tempLaser, 3.0f);
        Debug.Log("Laser fired");
    }


    // Use this for initialization
    void Start() {
        characterAnimator = GetComponent<Animator>();
        lightsaber = lightsaberParent.GetComponent<LightsaberMaster>();
        nav = GetComponent<NavMeshAgent>();
        timeDelay = Random.Range(3.0f, 4.0f);
    }

    // Update is called once per frame
    void Update() {

        faceTarget();
        turnTowardsTarget();
        pointGunAtTarget();

        // When lightsaber is ignighted, start firing
        if (Vector3.Distance(lightsaber.endPosition.localPosition, lightsaber.startPosition.localPosition) > .1 && !isAiming) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 1) {
                openFireOrder.enabled = true;
                if (timeDelay < 0) {
                    characterAnimator.SetBool("isCrouchFiring", true);
                    isAiming = true;
                    fireLazer();

                    timeDelay = Random.Range(.5f, 1.5f);
                }
            }
        }

        // If target gets too close, back up
        if (isAiming) {
            pointGunAtTarget();
            if (isTargetInsideDangerZone()) {
                //Vector3 newDestination = transform.position + -transform.forward * backUpDistance;
                //nav.SetDestination(newDestination);
                this.transform.Translate(0, 0, -baseSpeed);
                characterAnimator.SetBool("isBackingUp", true);
                isShooting = false;
                isBackingUp = true;
            } else {
                characterAnimator.SetBool("isBackingUp", false);
                characterAnimator.SetBool("isCrouchFiring", true);
                isShooting = true;
                isBackingUp = false;
            }
        }
        
        if (isShooting) {
            timeDelay -= Time.deltaTime;
            Debug.Log("Is shooting Time Delay: " + timeDelay);
            if (timeDelay < 0) {
                fireLazer();
                isShooting = false;

                timeDelay = Random.Range(.5f, 1.5f);
            }
        }
        
        if (isBackingUp) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 0) {
                fireLazer();

                isShooting = false;

                timeDelay = Random.Range(2f, 4f);
            }
        }

    }
}