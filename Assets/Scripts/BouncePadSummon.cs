using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePadSummon : MonoBehaviour {

    [SerializeField]
    private float upForce;

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

    private void Start() {
        if (!playSmokeAndSoundOnStart) return;
        Sound.Play(startSounds);
        Instantiate(smoke, smokeSpawnpoint.position, smokeSpawnpoint.rotation);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;
        Rigidbody rigidbody = vehicle.GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
        Sound.Play(useSound);
    }

}