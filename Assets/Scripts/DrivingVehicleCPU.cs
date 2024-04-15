using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DrivingVehicleCPU : Vehicle {

    [SerializeField]
    private WheelCollider flWheel, frWheel, blWheel, brWheel;
    private WheelCollider[] allWheels;
    private WheelCollider[] steeringWheels;

    [SerializeField]
    private Vector3 wheelVisualRotationOffset;

    [SerializeField]
    private float splineAnimateSpeed;
    private float GetRealSplineAnimateSpeed() => splineAnimateSpeed * (TrackManager.Instance.CPUSplineReverse ? -1 : 1);

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

    private SplineAnimate splineAnimate;
    private float splineAnimateDuration;

    protected override void Awake() {
        base.Awake();
        allWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        steeringWheels = new WheelCollider[] { flWheel, frWheel };

        splineAnimate = GetComponentInChildren<SplineAnimate>(true);
        splineAnimate.Container = TrackManager.Instance.CPUSpline;
        splineAnimate.StartOffset = TrackManager.Instance.CPUSplineStartPercent;
        splineAnimate.transform.SetParent(null, true);
        splineAnimate.MaxSpeed = Mathf.Abs(GetRealSplineAnimateSpeed());
        splineAnimateDuration = splineAnimate.Duration;
    }

    private void Update() {
        if (GameManager.Instance.CountdownOver) {
            float time = splineAnimate.NormalizedTime;
            time += GetRealSplineAnimateSpeed() / splineAnimateDuration * Time.deltaTime;
            time = Mathf.Repeat(time, 1);
            splineAnimate.NormalizedTime = time;
            splineAnimate.transform.position += Vector3.up * TrackManager.Instance.CPUSplineVerticalOffset;
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
            source.volume = engineVolumeAtSpeed.Evaluate(speed) * maxEngineVolume / GameInfo.CurrentPlayers;
        }
    }

    private void FixedUpdate() {
        if (GameManager.Instance.CountdownOver) {
            Vector3 force = Vector3.ClampMagnitude(splineAnimate.transform.position - transform.position, maxForce);
            force = Vector3.ProjectOnPlane(force, Vector3.up);
            _rigidbody.AddForce(force, ForceMode.Acceleration);
            if (_rigidbody.velocity != Vector3.zero) {
                _rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rigidbody.velocity), rotateSpeed * Time.deltaTime);
            }
        }
    }

}