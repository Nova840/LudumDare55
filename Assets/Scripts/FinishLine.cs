using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {

    private Dictionary<Rigidbody, bool> vehiclesInFinishLine = new Dictionary<Rigidbody, bool>();

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Rigidbody rigidbody = other.GetComponentInParent<Rigidbody>();
        if (vehiclesInFinishLine.ContainsKey(rigidbody)) return;
        Vehicle vehicle = rigidbody.GetComponent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;

        bool forward = Vector3.Angle(rigidbody.velocity, transform.forward) <= 90;
        vehiclesInFinishLine.Add(rigidbody, forward);
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Vehicle")) return;
        Rigidbody rigidbody = other.GetComponentInParent<Rigidbody>();
        if (!vehiclesInFinishLine.ContainsKey(rigidbody)) return;
        Vehicle vehicle = rigidbody.GetComponent<Vehicle>();
        if (vehicle.FinishLineTrigger != other) return;

        bool forward = Vector3.Angle(rigidbody.velocity, transform.forward) <= 90;
        bool countLapForward = vehiclesInFinishLine[rigidbody] && forward;
        bool countLapBackward = !vehiclesInFinishLine[rigidbody] && !forward;
        if (countLapForward) {
            GameInfo.GetPlayer(vehicle.PlayerIndex).Laps++;
        } else if (countLapBackward) {
            GameInfo.GetPlayer(vehicle.PlayerIndex).Laps--;
        }
        vehiclesInFinishLine.Remove(rigidbody);
    }

}