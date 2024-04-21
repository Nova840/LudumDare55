using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrivingVehicleCPU : Vehicle {

    [SerializeField]
    private WheelCollider flWheel, frWheel, blWheel, brWheel;
    private WheelCollider[] allWheels;
    private WheelCollider[] steeringWheels;

    [SerializeField]
    private Vector3 wheelVisualRotationOffset;

    [SerializeField]
    private float pathMoveSpeed;
    private float GetRealPathMoveSpeed() => pathMoveSpeed * (TrackManager.Instance.CPUPathReverse ? -1 : 1);

    [SerializeField]
    private float maxForce;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float maxSteerAngle;

    [SerializeField]
    private float steerMultiplier;

    [SerializeField]
    private float brakeTorque;

    [SerializeField]
    private AudioSource[] engineSounds;

    [SerializeField]
    private AnimationCurve engineVolumeAtSpeed;

    [SerializeField, Range(0, 1)]
    private float maxEngineVolume;

    [SerializeField]
    private Sound[] bumpSounds;

    [SerializeField]
    private float bumpSoundSpeedThreshold;

    [SerializeField]
    private float bumpSoundRepeatDelay;

    [SerializeField]
    private Transform followPoint;

    [SerializeField]
    private int[] vehiclesWithEngineSoundsInAir;

    private float pathNormalizedTime;

    private float timeLastBumpSoundPlayed = Mathf.NegativeInfinity;

    protected override void Awake() {
        base.Awake();
        allWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        steeringWheels = new WheelCollider[] { flWheel, frWheel };

        followPoint.SetParent(null, true);
        pathNormalizedTime = TrackManager.Instance.CPUPathStartPercent;
        followPoint.position = TrackManager.Instance.GetPathPoint(pathNormalizedTime);
    }

    protected override void Update() {
        base.Update();
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);
        if (GameManager.Instance.CountdownOver) {
            pathNormalizedTime += GetRealPathMoveSpeed() * Time.deltaTime / TrackManager.Instance.GetPathLength();
            pathNormalizedTime = Mathf.Repeat(pathNormalizedTime, 1);
            followPoint.position = TrackManager.Instance.GetPathPoint(pathNormalizedTime);
            followPoint.position += Vector3.up * TrackManager.Instance.CPUPathVerticalOffset;
        }

        foreach (WheelCollider wheel in allWheels) {
            wheel.brakeTorque = GameManager.Instance.CountdownOver ? 0 : brakeTorque;
            wheel.motorTorque = GameManager.Instance.CountdownOver ? .0001f : 0;
        }

        float targetSteerAngle = -Vector3.SignedAngle(_rigidbody.velocity, transform.forward, Vector3.up);
        float steerAngle = Mathf.Clamp(targetSteerAngle * steerMultiplier, -maxSteerAngle, maxSteerAngle);

        if (GameManager.Instance.CountdownOver) {
            foreach (WheelCollider wheel in steeringWheels) {
                wheel.steerAngle = steerAngle;
            }
        }

        foreach (WheelCollider wheel in allWheels) {
            wheel.GetWorldPose(out Vector3 position, out Quaternion rotation);
            foreach (Transform child in wheel.transform) {
                child.SetPositionAndRotation(position, rotation * Quaternion.Euler(wheelVisualRotationOffset));
            }
        }

        foreach (AudioSource source in engineSounds) {
            float speed = Vector3.ProjectOnPlane(_rigidbody.velocity, Vector3.up).magnitude;
            bool playInAir = vehiclesWithEngineSoundsInAir.Contains(player.vehicleIndex);
            float volume = engineVolumeAtSpeed.Evaluate(speed) * maxEngineVolume;
            if (!playInAir && allWheels.All(w => !w.isGrounded)) volume = 0;
            source.volume = volume / GameInfo.CurrentPlayers;
        }
    }

    private void FixedUpdate() {
        if (GameManager.Instance.CountdownOver) {
            Vector3 force = Vector3.ClampMagnitude(followPoint.position - transform.position, maxForce);
            force = Vector3.ProjectOnPlane(force, Vector3.up);
            _rigidbody.AddForce(force, ForceMode.Acceleration);
            if (_rigidbody.velocity != Vector3.zero) {
                _rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rigidbody.velocity), rotateSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (Time.time - timeLastBumpSoundPlayed >= bumpSoundRepeatDelay && collision.relativeVelocity.magnitude >= bumpSoundSpeedThreshold) {
            timeLastBumpSoundPlayed = Time.time;
            Sound.Play(bumpSounds[GameInfo.GetPlayer(PlayerIndex).vehicleIndex]);
        }
    }

}