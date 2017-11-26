using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRebelTrooperStanding : MonoBehaviour {

    public Transform target;
    public Transform head;
    public Transform gun;
    public Transform laserEmitter;
    public GameObject laserPrefab;
    public GameObject lightsaberParent;
    public AudioSource openFireOrder;
    public float baseSpeed = .04f;
    public float dangerZone = 4;
    public float backUpDistance = 2;
    public float laserSpeed = 800;
    public float shotGroupRadius = 2.5f;
    public float attackRange = 12;

    Animator characterAnimator;
    LightsaberMaster lightsaber;
    float timeDelay;
    bool isAiming = false;
    bool isShooting = false;
    bool isBackingUp = false;
    bool isEngaged = true;
    bool targetInRange = false;


    bool isLightsaberOn() { return lightsaberParent; }
    bool isTargetInsideDangerZone() { return getDistanceToTarget() < dangerZone; }
    bool isTargetInsideAttackRange() { return getDistanceToTarget() < attackRange; }

    // Getters
    Vector3 getDirectionOfTarget() { return target.position - this.transform.position; }    // will move to face target in all directions
    Vector3 getDirectionOfTargetRandom() { return target.position - this.transform.position + new Vector3(Random.Range(-shotGroupRadius, shotGroupRadius), Random.Range(-shotGroupRadius, shotGroupRadius), Random.Range(-shotGroupRadius, shotGroupRadius)) ; }
    Vector3 getDirectionOfTargetWithoutY() {                    // will move to face target, but remain on ground. Sets Y to Zero
        Vector3 directionOfTarget = getDirectionOfTarget();
        directionOfTarget.y = 0;
        return directionOfTarget;
    }
    float getDistanceToTarget() { return Vector3.Distance(target.position, this.transform.position); }

    // Methods
    void aim() {
        if (isLightsaberOn()) {
            characterAnimator.SetBool("isAiming", true);
        }
    }
    void openFire() {
        openFireOrder.enabled = true; ;
        if (!openFireOrder.isPlaying) {
            characterAnimator.SetBool("isFiring", true);
        }
    }
    void faceTarget() {
        head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(getDirectionOfTarget()), 0.2f);
    }
    void turnTowardsTarget() {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(getDirectionOfTargetWithoutY()), 0.4f);
    }
    void pointGunAtTarget() {
        gun.LookAt(target);
    }
    void pointEmitterAtTarget() {
        laserEmitter.LookAt(target);
        laserEmitter.rotation = Quaternion.Slerp(laserEmitter.rotation, Quaternion.LookRotation(getDirectionOfTargetRandom()), 0.4f);
    }
    void fireLazer() {
        GameObject tempLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation) as GameObject;
        Rigidbody tempLaserRigidBody = tempLaser.GetComponent<Rigidbody>();
        tempLaserRigidBody.AddForce(laserEmitter.transform.forward * laserSpeed);
        tempLaser.transform.Rotate(Vector3.left * 90);

        Destroy(tempLaser, 3.0f);
    }


    // Use this for initialization
    void Start() {
        characterAnimator = GetComponent<Animator>();
        lightsaber = lightsaberParent.GetComponent<LightsaberMaster>();
        timeDelay = Random.Range(3.0f, 4.0f);
    }

    // Update is called once per frame
    void Update() {
        faceTarget();
        turnTowardsTarget();
        pointGunAtTarget();
        pointEmitterAtTarget();
        targetInRange = isTargetInsideAttackRange();

        // When lightsaber is ignighted, start firing
        if (Vector3.Distance(lightsaber.endPosition.localPosition, lightsaber.startPosition.localPosition) > .1 && !isAiming) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 1) {
                openFireOrder.enabled = true;
                if (timeDelay < 0) {
                    isAiming = true;
                    if (targetInRange) {
                        characterAnimator.SetBool("isFiring", true);
                        fireLazer();
                    }
                    timeDelay = Random.Range(.5f, 1.5f);
                }
            }
        }

        // If target gets too close, back up
        if (isAiming) {
            pointEmitterAtTarget();
            if (isTargetInsideDangerZone()) {
                this.transform.Translate(0, 0, -baseSpeed);
                characterAnimator.SetBool("isBackingUp", true);
                isShooting = false;
                isBackingUp = true;
            } else {
                characterAnimator.SetBool("isBackingUp", false);
                isBackingUp = false;
                if (targetInRange) {
                    characterAnimator.SetBool("isFiring", true);
                    isShooting = true;
                }
            }
        }

        if (isShooting) {
            timeDelay -= Time.deltaTime;
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
