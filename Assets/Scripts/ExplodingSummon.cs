using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
    private bool playSmokeAndSoundOnStart;

    [SerializeField]
    private Transform smokeSpawnpoint;

    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private Transform explosionSpawnpoint;

    private void Start() {
        foreach (Animator animator in GetComponentsInChildren<Animator>(true)) {
            animator.Play(0, 0, Random.Range(0f, 1f));
        }
        if (playSmokeAndSoundOnStart) {
            Sound.Play(startSounds);
            Instantiate(smoke, smokeSpawnpoint.position, smokeSpawnpoint.rotation);
        }
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