using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    [SerializeField]
    private Vector3 rotateVector;

    [SerializeField]
    private Space space;

    private void Update() {
        transform.Rotate(rotateVector * Time.deltaTime, space);
    }

}