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

    [SerializeField]
    protected GameObject explodingSummonPrefab;

    [SerializeField]
    protected GameObject speedBoostSummonPrefab;

    [SerializeField]
    protected GameObject bouncePadSummonPrefab;

    [SerializeField]
    protected Transform explodingSummonSpawnpoint;

    [SerializeField]
    protected Transform speedBoostSummonSpawnpoint;

    [SerializeField]
    protected Transform bouncePadSummonSpawnpoint;

    [SerializeField]
    private float manaFillRate;

    [SerializeField]
    private GameObject smoke;

    [SerializeField]
    private Transform smokeSpawnpoint;

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

    protected virtual void Update() {
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);
        if (GameManager.Instance && GameManager.Instance.CountdownOver) {
            player.mana += manaFillRate * Time.deltaTime;
            player.mana = Mathf.Clamp01(player.mana);
        }
    }

    protected void Summon(GameObject summonPrefab, Transform spawnpoint) {
        GameInfo.Player player = GameInfo.GetPlayer(PlayerIndex);
        player.mana = 0;
        Quaternion rotation;
        if (transform.forward == Vector3.up) {
            rotation = Quaternion.LookRotation(-transform.up);
        } else {
            rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up));
        }
        Matrix4x4 vehicleSpace = Matrix4x4.TRS(transform.position, rotation, Vector3.one);
        Vector3 spawnPositionLocal = transform.InverseTransformPoint(spawnpoint.position);
        Vector3 spawnForwardLocal = transform.InverseTransformDirection(spawnpoint.forward);
        Vector3 spawnPosition = vehicleSpace.MultiplyPoint3x4(spawnPositionLocal);
        Vector3 spawnForward = vehicleSpace.MultiplyVector(spawnForwardLocal);
        Instantiate(summonPrefab, spawnPosition, Quaternion.LookRotation(spawnForward));
    }

    protected virtual void OnDestroy() {
        GameInfo.OnPlayerFinish -= OnPlayerFinish;
    }

    private void OnPlayerFinish(int playerIndex) {
        if (playerIndex == PlayerIndex) {
            Instantiate(smoke, smokeSpawnpoint.position, smokeSpawnpoint.rotation);
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
        Teleport(spawnpoint.position, spawnpoint.rotation);
        GameInfo.GetPlayer(PlayerIndex).SubtractLap();
    }

    protected void Teleport(Vector3 position, Quaternion rotation) {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.position = position;
        _rigidbody.rotation = rotation;
    }

}