using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingSummon : MonoBehaviour {

    [SerializeField]
    private float explodeForce;

    [SerializeField]
    private Sound[] startSounds;

    [SerializeField]
    private Sound useSound;

    [SerializeField]
    private GameObject smoke;

    [SerializeField]
    private Transform smokeSpawnpoint;

    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private Transform explosionSpawnpoint;

    private void Start() {
        if (!smoke) return;
        Sound.Play(startSounds);
        Instantiate(smoke, smokeSpawnpoint.position, smokeSpawnpoint.rotation);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;
        Rigidbody rigidbody = vehicle.GetComponent<Rigidbody>();
        Vector3 force = Vector3.ProjectOnPlane(rigidbody.position - transform.position, Vector3.up).normalized * explodeForce;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        Destroy(gameObject);
        Sound.Play(useSound);
        Instantiate(explosion, explosionSpawnpoint.position, explosionSpawnpoint.rotation);
    }

}