using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour {

    public static CameraManager Instance { get; private set; }

    [SerializeField]
    private float moveSmoothingSpeed, zoomSmoothingSpeed;

    [SerializeField]
    private AnimationCurve cameraDistanceAtBoundsSize;

    [SerializeField]
    private float stopTrackingYPosition;

    [SerializeField]
    private Transform cameraFollow;

    [SerializeField]
    private Rigidbody cameraRigidbody;

    private List<Vehicle> vehicles = new List<Vehicle>();
    public void AddVehicle(Vehicle vehicle) {
        vehicles.Add(vehicle);
        Snap();
    }

    private void Awake() {
        Instance = this;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Start") {
            Destroy(transform.GetChild(0).gameObject);
            TrackManager.Instance.PodiumCamera.gameObject.SetActive(false);
        } else if (sceneName == "End") {
            Destroy(transform.GetChild(0).gameObject);
            if (TrackManager.Instance.StartCamera) TrackManager.Instance.StartCamera.gameObject.SetActive(false);
        } else {
            cameraRigidbody.transform.SetParent(null, true);
            if (TrackManager.Instance.StartCamera) TrackManager.Instance.StartCamera.gameObject.SetActive(false);
            TrackManager.Instance.PodiumCamera.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {
        if (vehicles.Count == 0 || !cameraFollow) return;
        Bounds bounds = GetRelativeBounds();
        transform.position = Vector3.Lerp(transform.position, GetTargetPosition(bounds), moveSmoothingSpeed * Time.deltaTime);
        cameraFollow.localPosition = new Vector3(0, 0, Mathf.Lerp(cameraFollow.transform.localPosition.z, -GetTargetDistance(bounds), zoomSmoothingSpeed * Time.deltaTime));

        cameraRigidbody.velocity = (cameraFollow.position - cameraRigidbody.position) / Time.fixedDeltaTime;
    }

    private void Snap() {
        Bounds bounds = GetRelativeBounds();
        transform.position = GetTargetPosition(bounds);
        cameraFollow.localPosition = new Vector3(0, 0, -GetTargetDistance(bounds));
        cameraRigidbody.transform.position = cameraFollow.position;
    }

    private Bounds GetRelativeBounds() {
        Vector3 GetPlayerRelativePosition(Vehicle vehicle) {
            Vector3 position = vehicle.transform.position;
            return transform.InverseTransformPoint(position);
        }

        List<Vector3> relativePositions = new List<Vector3>();

        for (int i = 0; i < vehicles.Count; i++) {
            if (!vehicles[i] || vehicles[i].transform.position.y <= stopTrackingYPosition) continue;
            relativePositions.Add(GetPlayerRelativePosition(vehicles[i]));
        }

        if (relativePositions.Count == 0) {
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        Bounds bounds = new Bounds(relativePositions[0], Vector3.zero);
        for (int i = 1; i < relativePositions.Count; i++) {
            bounds.Encapsulate(relativePositions[i]);
        }
        return bounds;
    }

    private Vector3 GetTargetPosition(Bounds bounds) {
        Vector3 center = transform.TransformPoint(bounds.center);
        return center + transform.rotation * Vector3.back * bounds.extents.z;
    }

    private float GetTargetDistance(Bounds bounds) {
        float size = Mathf.Max(bounds.size.x, bounds.size.y);
        return cameraDistanceAtBoundsSize.Evaluate(size);
    }

}