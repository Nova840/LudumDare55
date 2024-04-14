using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringVehicle : Vehicle {

    [SerializeField]
    private float topSpeedGround;

    [SerializeField]
    private float topSpeedAir;

    [SerializeField]
    private float acceleration;

    [SerializeField]
    private float turningSpeed;

    [SerializeField]
    private float rotationSmoothingSpeedGround;

    [SerializeField]
    private float rotationMaxSpeedGround;

    [SerializeField]
    private float rotationSmoothingSpeedAir;

    [SerializeField]
    private float rotationMaxSpeedAir;

    [SerializeField]
    private float rayDistance;

    [SerializeField]
    private float springStrength;

    [SerializeField]
    private float springDamper;

    [SerializeField]
    private float vehicleMaxAngleForAir;

    private Vector3 velocityXZ = Vector3.zero;
    private Vector3 targetXZDirection;
    private Quaternion currentRotation;

    protected override void Awake() {
        base.Awake();
        targetXZDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        currentRotation = transform.rotation;
    }

    private void FixedUpdate() {
        bool didHit = GroundRay(out RaycastHit hit);

        Vector3 force = Vector3.zero;
        if (didHit) {
            force = GetUpwardForce(hit);
            _rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        bool isGrounded = didHit && force != Vector3.zero && Vector3.Angle(hit.normal, Vector3.up) <= vehicleMaxAngleForAir;

        Vector2 moveVector = Vector2.zero;
        if (GameManager.Instance.CountdownOver) {
            moveVector = InputManager.GetMoveVector(GameInfo.GetPlayer(PlayerIndex).controller);
        }
        Vector3 moveDirection = new Vector3(moveVector.x, 0, moveVector.y);

        if (moveDirection != Vector3.zero) {
            targetXZDirection = moveDirection;
        }
        Vector3 hitNormal = isGrounded ? hit.normal : Vector3.up;
        Quaternion targetRotation = Quaternion.LookRotation(hitNormal, targetXZDirection) * Quaternion.Euler(-90, 0, 180);
        float maxSpeed = isGrounded ? rotationMaxSpeedGround : rotationMaxSpeedAir;
        targetRotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxSpeed * Time.deltaTime);
        float smoothingSpeed = isGrounded ? rotationSmoothingSpeedGround : rotationSmoothingSpeedAir;
        transform.rotation = currentRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothingSpeed * Time.deltaTime);

        float topSpeed = isGrounded ? topSpeedGround : topSpeedAir;
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
    private Vector3 GetUpwardForce(RaycastHit hit) {
        Vector3 springDir = Vector3.up;
        Vector3 worldVel = _rigidbody.velocity;
        float offset = rayDistance - hit.distance;
        float vel = Vector3.Dot(springDir, worldVel);
        float force = (offset * springStrength) - (vel * springDamper);
        force = Mathf.Max(force, 0);
        return springDir * force;
    }

    protected override void Respawn() {
        base.Respawn();
        currentRotation = transform.rotation;
        targetXZDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        velocityXZ = Vector3.zero;
    }

}