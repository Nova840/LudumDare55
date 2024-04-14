using System.Collections;
using System.Collections.Generic;
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

    private float steerAngle = 0;

    protected override void Awake() {
        base.Awake();
        allWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        steeringWheels = new WheelCollider[] { flWheel, frWheel };
        drivingWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
    }

    private void Update() {
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);

        float steering = InputManager.GetMoveVector(player.controller).x;
        float accelerate;
        if (player.controller < 0) {
            accelerate = InputManager.GetKeyboardForward(player.controller) ? 1 : 0;
        } else {
            accelerate = InputManager.GetGamepadAccelerate(player.controller);
        }
        float reverse;
        if (player.controller < 0) {
            reverse = InputManager.GetKeyboardBackward(player.controller) ? 1 : 0;
        } else {
            reverse = InputManager.GetGamepadBrake(player.controller);
        }
        float brake = 0;
        if (accelerate > 0 && reverse > 0) {
            brake = (accelerate + reverse) / 2f;
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
    }

}