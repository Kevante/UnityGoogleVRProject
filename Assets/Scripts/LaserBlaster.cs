using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlaster : MonoBehaviour {

    [Space(15)]

    [Header("Target")]
    [SerializeField()]
    Transform target;

    [Space(20)]

    [Header("Accuracy")]
    [SerializeField()]
    [Range(0, 10)]
    float shotGroup = 3;

    [Space(15)]

    [Header("Laser")]
    [SerializeField()]
    GameObject laserPrefab;
    [SerializeField()]
    [Range(0, 2000)]
    float laserSpeed = 800;

    [Space(15)]

    [Header("Muzzle Flash")]
    [SerializeField()]
    bool muzzleFlash;
    [SerializeField()]
    LensFlare flashFlare;
    [SerializeField()]
    [Range(0, .5f)]
    float lens_min = .4f;
    [SerializeField()]
    [Range(.5f, 1f)]
    float lens_max = .7f;
    [SerializeField()]
    Light flashLight;
    [SerializeField()]
    [Tooltip("If checked, the Muzzle Flash light intensity will match the Muzzle Flash Lens Flare intensity")]
    bool matchFlare;
    [SerializeField()]
    [Range(0, 10)]
    float lightRange = 2f;
    [SerializeField()]
    [Range(0, 10)]
    float lightIntensity = 1f;


    [Space(15)]

    [Header("Sounds")]
    [SerializeField()]
    bool sound;
    [SerializeField()]
    AudioClip dischargeSound;

    [Space(15)]
    [Header("Components")]
    [SerializeField()]
    Transform laserEmitter;



    Quaternion defaultEmitterRotation;

    float timeDelay;

    float flashDelay;
    Collider[] allColliders;
    AudioSource audioSource;
    


    Vector3 getRandomDirection() { return new Vector3(Random.Range(-shotGroup, shotGroup), Random.Range(-shotGroup, shotGroup), Random.Range(-shotGroup, shotGroup)); }
    Vector3 getDirectionOfTargetRandom() { return target.position - this.transform.position + new Vector3(Random.Range(-shotGroup, shotGroup), Random.Range(-shotGroup, shotGroup), Random.Range(-shotGroup, shotGroup)); }


    public void pointGunAtTarget() {
        GetComponent<Transform>().LookAt(target);
    }
    public void pointEmitter() {
        laserEmitter.LookAt(target);
        laserEmitter.rotation = Quaternion.Slerp(laserEmitter.rotation, Quaternion.LookRotation(getDirectionOfTargetRandom()), .1f);
    }
    public void fire() {
        pointEmitter();

        if (muzzleFlash) {
            flashFlare.enabled = true;
            flashLight.enabled = true;
            flashFlare.brightness = Random.Range(lens_min, lens_max);
            flashLight.range = lightRange;
            flashLight.intensity = lightIntensity;

            if (matchFlare) {
                flashLight.intensity = flashFlare.brightness * 100;
            }

            flashDelay = .1f;
        }

        if (sound) {
            audioSource.clip = dischargeSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        GameObject tempLaser = Instantiate(laserPrefab, laserEmitter.position, laserEmitter.rotation) as GameObject;
        //Physics.IgnoreCollision(tempLaser.GetComponent<Collider>(), GetComponent<Collider>());
        allColliders = GetComponents<Collider>();
        foreach (Collider coll in allColliders) {
            Physics.IgnoreCollision(tempLaser.GetComponent<Collider>(), coll);
        }

        Rigidbody tempLaserRigidBody = tempLaser.GetComponent<Rigidbody>();
        tempLaser.transform.Rotate(Vector3.left * 90);
        tempLaser.transform.Rotate(Vector3.forward * 180);
        tempLaserRigidBody.AddForce(laserEmitter.transform.forward * laserSpeed, ForceMode.Force);
        
        laserEmitter.rotation = defaultEmitterRotation;

        Destroy(tempLaser, laserSpeed/500f);
    }
    
    // Use this for initialization
    void Start () {
        laserEmitter = transform.Find("LaserEmitter");
        defaultEmitterRotation = laserEmitter.rotation;
        audioSource = laserEmitter.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        timeDelay = 3f;
        flashDelay = .1f;
    }

    // Update is called once per frame
    void Update() {

        if (flashFlare.enabled) {
            flashDelay -= Time.deltaTime;
            if(flashDelay < 0) {
                flashFlare.enabled = false;
                flashLight.enabled = false;
            }
        }
    }
}
