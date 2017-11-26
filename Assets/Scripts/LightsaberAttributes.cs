using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberAttributes : MonoBehaviour {

    public GameObject hitFXGameObject;
    public GameObject playerCollider;
    public GameObject pointLightGameObject;

    private bool on;
    private float timeSinceLastCollision;
    private Vector3 lastPoint;

    private void Start()
    {
        hitFXGameObject.GetComponent<ParticleSystem>().Stop();
        pointLightGameObject.SetActive(false);
        Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider.GetComponent<Collider>());
        timeSinceLastCollision += Time.deltaTime;
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Metal")
        {
            hitFXGameObject.SetActive(true);
            hitFXGameObject.GetComponent<ParticleSystem>().Play();
            hitFXGameObject.transform.position = other.contacts[0].point;
            pointLightGameObject.SetActive(true);
        }
        timeSinceLastCollision = 0f;
    }

    private void OnCollisionExit(Collision collision)
    {
        pointLightGameObject.SetActive(false);
        hitFXGameObject.GetComponent<ParticleSystem>().Stop();
    }
}
