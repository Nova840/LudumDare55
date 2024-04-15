using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostSummon : MonoBehaviour {

    [SerializeField]
    private float forwardForce;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;
        Rigidbody rigidbody = vehicle.GetComponent<Rigidbody>();
        Vector3 force = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * forwardForce;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

}