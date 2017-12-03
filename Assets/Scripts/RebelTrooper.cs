using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebelTrooper : MonoBehaviour {

    [Space(15)]

    [Header("Default Position")]
    [SerializeField()]
    bool kneeling;
    [SerializeField()]
    bool standing;

    [Space(15)]

    [Header("Default Position")]
    [SerializeField()]
    [Tooltip("Drag 'head' GameObject here")]
    Transform head;

    [Space(15)]

    [Header("Target")]
    [SerializeField()]
    Transform target;
    [SerializeField()]
    GameObject targetLightsaber;


    [Space(15)]
    
    [Header("Weapon")]
    [SerializeField()]
    [Tooltip("Drag 'gun' GameObject here")]
    GameObject gun;


    [Space(15)]

    [Header("Rate of Fire")]
    [SerializeField()]
    [Tooltip("Amount of seconds per shot. Will choose a random number of seconds in the min and max range before firing each shot")]
    [Range(0f, 10f)]
    float rateMin = 3f;
    [SerializeField()]
    [Tooltip("Amount of seconds per shot. Will choose a random number of seconds in the min and max range before firing each shot")]
    [Range(0f, 10f)]
    float rateMax = 4f;


    [Space(15)]

    [Header("Sounds")]
    [SerializeField()]
    bool giveOpenFireOrder;
    [SerializeField()]
    AudioClip openFire;


    [Space(10)]

    [SerializeField()]
    bool callForHelp;
    [SerializeField()]
    AudioClip help;


    [Space(15)]

    [SerializeField()]
    AudioClip hit;
    [SerializeField()]
    AudioClip death;
    [SerializeField()]
    AudioClip choke;


    [Space(20)]
    
    public float baseSpeed = .04f;
    public float dangerZone = 3;
    public float backUpDistance = 2;


    Animator characterAnimator;
    float timeDelay;
    bool isAiming = false;
    bool isShooting = false;
    bool isBackingUp = false;
    bool isEngaged = true;
    bool isAlive = true;
    bool openFireCalled = false;

    AudioSource audioSource;

    LaserBlaster gunScript;
    Dynamic_Laser saberScript;


    bool isTargetInsideDangerZone() { return getDistanceToTarget() < dangerZone; }

    // Getters
    Vector3 getDirectionOfTarget() { return target.position - this.transform.position; }    // will move to face target in all directions
    Vector3 getDirectionOfTargetWithoutY() {                    // will move to face target, but remain on ground. Sets Y to Zero
        Vector3 directionOfTarget = getDirectionOfTarget();
        directionOfTarget.y = 0;
        return directionOfTarget;
    }
    float getDistanceToTarget() { return Vector3.Distance(target.position, this.transform.position); }

    // Methods
    public void kill() {
        if (isAlive) {
            characterAnimator.SetTrigger("isDead");
            isAlive = false;
            Destroy(gameObject, 3);
        }
    }


    void faceTarget() {
        head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(getDirectionOfTarget()), 0.2f);
    }
    void turnTowardsTarget() {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(getDirectionOfTargetWithoutY()), 0.4f);
    }
    void pointGunAtTarget() {
        gunScript.pointGunAtTarget();
    }
    void aim() {
         characterAnimator.SetBool("isCrouchAiming", true);
    }
    void fire() {
        gunScript.fire();
    }
    void openFireOrder() {
        audioSource.loop = false;
        audioSource.clip = openFire;
        audioSource.Play();

        openFireCalled = true;
    }


    // Use this for initialization
    void Start() {

        characterAnimator = GetComponent<Animator>();
        audioSource = head.GetComponent<AudioSource>();
        gunScript = gun.GetComponent<LaserBlaster>();
        saberScript = targetLightsaber.GetComponent<Dynamic_Laser>();

        audioSource.playOnAwake = false;

        timeDelay = Random.Range(3.0f, 4.0f);
        timeDelay = 3f;
    }

    // Update is called once per frame
    void Update() {
        faceTarget();
        turnTowardsTarget();
        gunScript.pointGunAtTarget();
        gunScript.pointEmitter();
        

        // When lightsaber is ignighted, start firing
        if ((saberScript.active) && !isAiming && isAlive) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 1) {
                if (giveOpenFireOrder && !openFireCalled)
                    openFireOrder();
                if (timeDelay < 0) {
                    characterAnimator.SetBool("isCrouchFiring", true);
                    isAiming = true;
                    fire();

                    timeDelay = Random.Range(rateMin, rateMax);
                }
            }
        }

        // If target gets too close, back up
        if (isAiming && isAlive) {
            gunScript.pointEmitter();
            if (isTargetInsideDangerZone()) {
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

        if (isShooting && isAlive) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 0) {
                fire();
                isShooting = false;
                timeDelay = Random.Range(rateMin, rateMax);
            }
        }

        if (isBackingUp && isAlive) {
            timeDelay -= Time.deltaTime;
            if (timeDelay < 0) {
                fire();
                isShooting = false;
                timeDelay = Random.Range(rateMin, rateMax) + 2;
            }
        }
    }
}
