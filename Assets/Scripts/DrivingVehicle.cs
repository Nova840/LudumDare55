using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrivingVehicle : Vehicle {

    [SerializeField]
    private WheelCollider flWheel, frWheel, blWheel, brWheel;
    private WheelCollider[] allWheels;
    private WheelCollider[] steeringWheels;
    private WheelCollider[] drivingWheels;

    [SerializeField]
    private Vector3 wheelVisualRotationOffset;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float maxSteerAngle;

    [SerializeField]
    private float motorTorque;

    [SerializeField]
    private float brakeTorque;

    [SerializeField]
    private float steerSmoothSpeed;

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

    [SerializeField, Range(0, 180)]
    private float moveCenterOfMassAngle;

    [SerializeField]
    private float moveCenterOfMassWhenFlippedSpeed;

    [SerializeField]
    private int[] vehiclesWithEngineSoundsInAir;

    private Vector3 initialCenterOfMass;

    private float timeLastBumpSoundPlayed = Mathf.NegativeInfinity;

    private float steerAngle = 0;

    protected override void Awake() {
        base.Awake();
        allWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        steeringWheels = new WheelCollider[] { flWheel, frWheel };
        drivingWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        initialCenterOfMass = _rigidbody.centerOfMass;
    }

    protected override void Update() {
        base.Update();
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);
        bool countdownOver = GameManager.Instance.CountdownOver;
        float steering = countdownOver ? InputManager.GetMoveVector(player.controller).x : 0;
        float accelerate = countdownOver ? InputManager.GetAccelerate(player.controller) : 0;
        float reverse = countdownOver ? InputManager.GetReverse(player.controller) : 0;
        float brake = InputManager.GetBrake(player.controller);
        if (!countdownOver) {
            brake = 1;
        } else if (accelerate > 0 && reverse > 0) {
            brake += (accelerate + reverse) / 2f;
            brake = Mathf.Clamp01(brake);
        }

        if (Vector3.Angle(transform.up, Vector3.up) >= moveCenterOfMassAngle) {
            _rigidbody.centerOfMass += Vector3.down * (moveCenterOfMassWhenFlippedSpeed * Time.deltaTime);
        } else {
            _rigidbody.centerOfMass = initialCenterOfMass;
        }

        steerAngle = Mathf.Lerp(steerAngle, steering * maxSteerAngle, steerSmoothSpeed * Time.deltaTime);
        foreach (WheelCollider wheel in steeringWheels) {
            wheel.steerAngle = steerAngle;
        }

        foreach (WheelCollider wheel in allWheels) {
            wheel.brakeTorque = brake * brakeTorque;
        }

        foreach (WheelCollider wheel in drivingWheels) {
            float speed = Mathf.Abs(wheel.rpm) * wheel.radius * 2 * Mathf.PI;
            if (speed >= maxSpeed || wheel.brakeTorque > 0) {
                wheel.motorTorque = 0;
            } else {
                wheel.motorTorque = (accelerate - reverse) * motorTorque;
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

    private void OnCollisionEnter(Collision collision) {
        if (Time.time - timeLastBumpSoundPlayed >= bumpSoundRepeatDelay && collision.relativeVelocity.magnitude >= bumpSoundSpeedThreshold) {
            timeLastBumpSoundPlayed = Time.time;
            Sound.Play(bumpSounds[GameInfo.GetPlayer(PlayerIndex).vehicleIndex]);
        }
    }

}