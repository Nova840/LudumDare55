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
    private Renderer[] turtleRenderers;

    [SerializeField]
    private Renderer[] hatRenderers;

    [SerializeField]
    private Transform meshContainer;

    [SerializeField]
    private Outline.Mode outlineMode;

    [SerializeField]
    private float outlineWidth;

    protected Rigidbody _rigidbody;
    private Outline outline;

    public int PlayerIndex { get; private set; }

    public void Initialize(int playerIndex) {
        PlayerIndex = playerIndex;
    }

    protected virtual void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start() {
        CameraManager.Instance.AddVehicle(this);
        GameInfo.OnPlayerFinish += OnPlayerFinish;
        StartCoroutine(RespawnCheckRoutine());
        foreach (Renderer r in turtleRenderers) {
            r.materials[0].color = GameInfo.GetPlayer(PlayerIndex).color;
        }
        foreach (Renderer r in hatRenderers) {
            r.materials[1].color = GameInfo.GetPlayer(PlayerIndex).color;
        }
        meshContainer.GetChild(GameInfo.GetPlayer(PlayerIndex).vehicleIndex).gameObject.SetActive(true);

        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = outlineMode;
        outline.OutlineColor = GameInfo.GetPlayer(PlayerIndex).color;
        outline.OutlineWidth = outlineWidth;
    }

    protected virtual void OnDestroy() {
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