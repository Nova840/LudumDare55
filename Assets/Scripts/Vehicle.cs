using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle : MonoBehaviour {

    [SerializeField]
    private float topSpeed;

    [SerializeField]
    private float acceleration;

    [SerializeField]
    private float turningSpeed;

    [SerializeField]
    private float rayDistance;

    [SerializeField]
    private float springStrength;

    [SerializeField]
    private float springDamper;

    private Rigidbody _rigidbody;

    private Vector3 velocityXZ = Vector3.zero;
    private Vector3 targetXZDirection;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        targetXZDirection = transform.forward;
    }

    private void FixedUpdate() {
        Vector2 inputVector = new Vector2(Keyboard.current.dKey.value - Keyboard.current.aKey.value, Keyboard.current.wKey.value - Keyboard.current.sKey.value);
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDirection != Vector3.zero) {
            targetXZDirection = Vector3.RotateTowards(transform.forward, moveDirection, turningSpeed * Time.deltaTime, 0);
        }
        transform.rotation = Quaternion.LookRotation(targetXZDirection);

        AddUpwardForce();

        float speed = moveDirection == Vector3.zero ? 0 : topSpeed;
        velocityXZ = Vector3.Lerp(velocityXZ, transform.forward * speed, acceleration * Time.deltaTime);
        _rigidbody.velocity = new Vector3(velocityXZ.x, _rigidbody.velocity.y, velocityXZ.z);
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private bool GroundRay(out RaycastHit hit) {
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.red, 0, true);
        return Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, ~LayerMask.GetMask("Vehicle"), QueryTriggerInteraction.Ignore);
    }

    //https://youtu.be/CdPYlj5uZeI?si=DAdpflGTnhWeMEBA
    private void AddUpwardForce() {
        if (GroundRay(out RaycastHit hit)) {
            Vector3 springDir = Vector3.up;
            Vector3 worldVel = _rigidbody.velocity;
            float offset = rayDistance - hit.distance;
            float vel = Vector3.Dot(springDir, worldVel);
            float force = (offset * springStrength) - (vel * springDamper);
            _rigidbody.AddForce(springDir * force, ForceMode.Acceleration);
        }
    }

}