using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePadSummon : MonoBehaviour {

    [SerializeField]
    private float upForce;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Vehicle vehicle = other.GetComponentInParent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;
        Rigidbody rigidbody = vehicle.GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.up * upForce, ForceMode.VelocityChange);
    }

}