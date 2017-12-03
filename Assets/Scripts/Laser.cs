using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    [Space(15)]

    [Header("Flickering")]
    [SerializeField()]
    bool flickering;
    [SerializeField()]
    [Range(0.01f, 0.05f)]
    float flick_rate = 0.025f;
    [SerializeField()]
    [Range(0.1f, 0.9f)]
    float flick_min = 0.5f;
    [SerializeField()]
    Light laser_light;
    [SerializeField()]
    [Range(1, 8)]
    float light_multiplier = 2f;

    [Space(15)]

    [Header("Beam Glow")]
    [SerializeField()]
    bool glowing;
    [SerializeField()]
    Light beamGlow;
    [SerializeField()]
    [Range(0, 10)]
    float brightness;

    [Space(15)]

    [Header("Impact Sparks")]
    [SerializeField()]
    LensFlare SparksLensFlare;
    [SerializeField()]
    [Range(0, 50)]
    float lens_min = 50f;
    [SerializeField()]
    [Range(50, 100)]
    float lens_max = 70f;
    [SerializeField()]
    Light SparksLight;
    [SerializeField()]
    [Range(0, 10)]
    float lightRange = 5f;
    [SerializeField()]
    [Range(0, 10)]
    float lightIntensity = 1f;
    [SerializeField()]
    ParticleSystem spark_particle;


    [Space(15)]

    [Header("Sounds")]
    [SerializeField()]
    AudioClip laserHum;
    [SerializeField()]
    AudioClip impactSound;
    [SerializeField()]
    AudioClip lightningSound;

    [Space(20)]

    [Header("Beam Particles")]
    [SerializeField()]
    ParticleSystem lightning_particle;
    [SerializeField()]
    ParticleSystem smoke_particle;


    public Transform laserOrigin;
    public Transform laserEnd;

    AudioSource audioSource;

    bool active;

    float flick_timer;

    public GameObject beam;
    Collider laserCollider;
    Rigidbody laserRigidBody;

    RaycastHit rhit_laser;

    public Rigidbody getRigidBody() {
        return beam.GetComponent<Rigidbody>();
    }


    void Start () {
        
        laserRigidBody = beam.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        _Enable();

    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void FixedUpdate() {
        if (!flickering || !active)
            return;

        flick_timer += Time.deltaTime;
        if (flick_timer > flick_rate) {
            flick_timer = 0f;
            float r = Random.Range(flick_min, 1f);

            if (laser_light != null)
                laser_light.intensity = r * light_multiplier;
        }
    }

    void OnTriggerEnter(Collider coll) {
        if (coll.GetComponent<Laser_Collider>() != null) {
            if (SparksLensFlare != null)
                SparksLensFlare.enabled = true;
            audioSource.PlayOneShot(impactSound);
        }
    }

    void OnTriggerStay(Collider coll) {
        if (coll.GetComponent<Laser_Collider>() != null) {
            if (Physics.Linecast(laserEnd.position, laserOrigin.position, out rhit_laser)) {
                if (SparksLensFlare != null) {
                    SparksLensFlare.brightness = Random.Range(lens_min, lens_max);
                    SparksLensFlare.transform.position = rhit_laser.point;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("enemy") != null) {
            if (other.CompareTag("enemy"))
                _Disable();
        }
    }
    

    public void _Enable() {
        if (active)
            return;

        active = true;

        audioSource.loop = true;
        audioSource.PlayOneShot(laserHum);
        audioSource.PlayOneShot(lightningSound);
        
        if (laser_light != null) {
            laser_light.enabled = true;
        }
        if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
            lightning_particle.Play();
        if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
            smoke_particle.Play();
        if (beamGlow != null) {
            beamGlow.enabled = glowing;
            beamGlow.range = lightRange;
            beamGlow.intensity = lightIntensity;
        }
    }

    public void _Disable() {
        if (!active)
            return;

        active = false;

        audioSource.loop = false;

        if (laser_light != null) {
            laser_light.enabled = false;
        }
        if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
            lightning_particle.Stop();
        if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
            smoke_particle.Stop();
        if (SparksLensFlare != null)
            SparksLensFlare.enabled = false;
        if (glowing != null)
            beamGlow.enabled = false;
    }
}
