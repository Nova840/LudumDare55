using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingSummon : MonoBehaviour {

    [SerializeField]
    private float explodeForce;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;
        Rigidbody rigidbody = vehicle.GetComponent<Rigidbody>();
        Vector3 force = Vector3.ProjectOnPlane(rigidbody.position - transform.position, Vector3.up).normalized * explodeForce;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        Destroy(gameObject);
    }

}