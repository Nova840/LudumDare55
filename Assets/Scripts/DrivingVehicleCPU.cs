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
    private float pathAheadDistance;
    private float GetRealPathAheadDistance() {
        float speed = TrackManager.Instance.CPUPathReverse ? -pathAheadDistance : pathAheadDistance;
        return speed;
    }

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

    [SerializeField]
    private float respawnStuckDistance;

    [SerializeField]
    private float respawnStuckTime;

    [SerializeField]
    private float respawnStuckCheckInterval;

    [SerializeField]
    private float useSummonMaxDelay;

    [SerializeField, Range(0, 180)]
    private float moveCenterOfMassAngle;

    [SerializeField]
    private float moveCenterOfMassWhenFlippedSpeed;

    private float initialDrag;
    private Vector3 initialCenterOfMass;

    private int RespawnCheckPositionsMaxSize => Mathf.FloorToInt(respawnStuckTime / respawnStuckCheckInterval);

    private List<Vector3> respawnCheckPositions = new List<Vector3>();

    private float pathNormalizedTime;

    private float timeLastBumpSoundPlayed = Mathf.NegativeInfinity;

    private bool fullManaLastFrame = false;

    protected override void Awake() {
        base.Awake();
        allWheels = new WheelCollider[] { flWheel, frWheel, blWheel, brWheel };
        steeringWheels = new WheelCollider[] { flWheel, frWheel };

        followPoint.SetParent(null, true);
        ResetFollowPoint();
        initialDrag = _rigidbody.drag;
        initialCenterOfMass = _rigidbody.centerOfMass;
    }

    protected override void Update() {
        base.Update();
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);
        if (GameManager.Instance.CountdownOver) {
            pathNormalizedTime = TrackManager.Instance.GetClosestTimeOnPath(transform.position);
            pathNormalizedTime += GetRealPathAheadDistance() / TrackManager.Instance.GetPathLength();
            pathNormalizedTime = Mathf.Repeat(pathNormalizedTime, 1);
            SetFollowPoint();
        }

        if (Vector3.Angle(transform.up, Vector3.up) >= moveCenterOfMassAngle) {
            _rigidbody.centerOfMass += Vector3.down * (moveCenterOfMassWhenFlippedSpeed * Time.deltaTime);
        } else {
            _rigidbody.centerOfMass = initialCenterOfMass;
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

        bool fullMana = player.mana == 1;
        if (fullMana && !fullManaLastFrame) {
            int r = Random.Range(0, 3);
            if (r == 0) {
                StartCoroutine(SummonAfterDelay(explodingSummonPrefab, explodingSummonSpawnpoint));
            } else if (r == 1) {
                StartCoroutine(SummonAfterDelay(bouncePadSummonPrefab, bouncePadSummonSpawnpoint));
            } else if (r == 2) {
                StartCoroutine(SummonAfterDelay(speedBoostSummonPrefab, speedBoostSummonSpawnpoint));
            }
        }
        fullManaLastFrame = fullMana;
    }

    protected override void Start() {
        base.Start();
        StartCoroutine(CheckRespawnRoutine());
    }

    private void FixedUpdate() {
        if (GameManager.Instance.CountdownOver) {
            bool grounded = allWheels.All(w => w.isGrounded);
            _rigidbody.drag = grounded ? initialDrag : 0;
            if (grounded) {
                _rigidbody.angularVelocity = Vector3.zero;
                Vector3 force = followPoint.position - transform.position;
                force = Vector3.ProjectOnPlane(force, Vector3.up);
                force = Vector3.ClampMagnitude(force, maxForce);
                _rigidbody.AddForce(force, ForceMode.Acceleration);
                if (_rigidbody.velocity != Vector3.zero) {
                    _rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rigidbody.velocity), rotateSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (Time.time - timeLastBumpSoundPlayed >= bumpSoundRepeatDelay && collision.relativeVelocity.magnitude >= bumpSoundSpeedThreshold) {
            timeLastBumpSoundPlayed = Time.time;
            Sound.Play(bumpSounds[GameInfo.GetPlayer(PlayerIndex).vehicleIndex]);
        }
    }

    private void SetFollowPoint() {
        followPoint.position = TrackManager.Instance.GetPathPoint(pathNormalizedTime);
        followPoint.position += Vector3.up * TrackManager.Instance.CPUPathVerticalOffset;
    }

    private IEnumerator CheckRespawnRoutine() {
        while (true) {
            respawnCheckPositions.Add(transform.position);
            if (respawnCheckPositions.Count > RespawnCheckPositionsMaxSize) {
                respawnCheckPositions.RemoveAt(0);
            }
            if (ShouldRespawn()) {
                pathNormalizedTime = TrackManager.Instance.GetClosestTimeOnPath(transform.position);
                Vector3 position = TrackManager.Instance.GetPathPoint(pathNormalizedTime);
                Vector3 facingPosition = TrackManager.Instance.GetPathPoint(pathNormalizedTime + GetRealPathAheadDistance() / TrackManager.Instance.GetPathLength());
                Quaternion rotation = Quaternion.LookRotation(facingPosition - position);
                Teleport(position, rotation);
                respawnCheckPositions.Clear();
            }
            yield return new WaitForSeconds(respawnStuckCheckInterval);
        }
    }

    private bool ShouldRespawn() {
        if (respawnCheckPositions.Count < RespawnCheckPositionsMaxSize) return false;
        for (int i = 0; i < respawnCheckPositions.Count; i++) {
            for (int j = i; j < respawnCheckPositions.Count; j++) {
                if (i == j) continue;
                if (Vector3.Distance(respawnCheckPositions[i], respawnCheckPositions[j]) <= respawnStuckDistance) continue;
                return false;
            }
        }
        return true;
    }

    protected override void Respawn() {
        base.Respawn();
        ResetFollowPoint();
    }

    private void ResetFollowPoint() {
        pathNormalizedTime = TrackManager.Instance.CPUPathStartPercent;
        followPoint.position = TrackManager.Instance.GetPathPoint(pathNormalizedTime);
    }

    private IEnumerator SummonAfterDelay(GameObject summoonPrefab, Transform spawnpoint) {
        float randomDelay = Random.Range(0, useSummonMaxDelay);
        if (randomDelay > 0) yield return new WaitForSeconds(randomDelay);
        Summon(summoonPrefab, spawnpoint);
    }

}