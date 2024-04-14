using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour {

    [SerializeField]
    private Collider finishLineTrigger;
    public Collider FinishLineTrigger => finishLineTrigger;

    [SerializeField]
    private float respawnYPosition;

    [SerializeField]
    private float respawnCheckInterval;

    [SerializeField]
    private Renderer turtleRenderer;

    protected Rigidbody _rigidbody;

    public int PlayerIndex { get; private set; }

    public void Initialize(int playerIndex) {
        PlayerIndex = playerIndex;
    }

    protected virtual void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        CameraManager.Instance.AddVehicle(this);
        GameInfo.OnPlayerFinish += OnPlayerFinish;
        StartCoroutine(RespawnCheckRoutine());
        turtleRenderer.material.color = GameInfo.GetPlayer(PlayerIndex).color;
    }

    private void OnDestroy() {
        GameInfo.OnPlayerFinish -= OnPlayerFinish;
    }

    private void OnPlayerFinish(int playerIndex) {
        if (playerIndex == PlayerIndex) {
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnCheckRoutine() {
        while (true) {
            yield return new WaitForSeconds(respawnCheckInterval);
            if (transform.position.y <= respawnYPosition) {
                Respawn();
            }
        }
    }

    protected virtual void Respawn() {
        Transform spawnpoint = Spawnpoints.Instance.GetSpawnpoint(Random.Range(0, Spawnpoints.Instance.GetNumberOfSpawnpoints()));
        _rigidbody.position = spawnpoint.position;
        transform.rotation = spawnpoint.rotation;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        GameInfo.GetPlayer(PlayerIndex).SubtractLap();
    }

}