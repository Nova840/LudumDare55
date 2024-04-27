using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject vehiclePrefab;

    [SerializeField]
    private GameObject cpuVehiclePrefab;

    [SerializeField]
    private TMP_Text countdownText;

    [SerializeField]
    private int countdownTime;

    [SerializeField]
    private float raceEndDelay;

    [SerializeField]
    private Sound raceStartSound;

    [SerializeField]
    private Sound raceEndSound;

    private float timeStarted = Mathf.NegativeInfinity;
    public float TimeElapsed => Time.time - timeStarted - countdownTime;
    public bool CountdownOver { get; private set; } = false;

    private int numPlayersFinished = 0;

    private void Awake() {
        Instance = this;
        if (GameInfo.StartSceneLoaded) {
            SceneManager.LoadScene(GameInfo.LevelName, LoadSceneMode.Additive);
        }
        GameInfo.OnPlayerFinish += OnPlayerFinish;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy() {
        GameInfo.OnPlayerFinish -= OnPlayerFinish;
    }

    private void Start() {
        Sound.Play(raceStartSound);
        StartCoroutine(Countdown());
        GameInfo.ForEachPlayer(p => {
            p?.Reset();
        });
        if (GameInfo.StartSceneLoaded) {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameInfo.LevelName));//not active in awake
        }

        timeStarted = Time.time;

        GameInfo.Player[] playersRandomized = GameInfo.GetPlayers().Where(p => p != null).ToArray();
        playersRandomized.Shuffle();
        for (int i = 0; i < playersRandomized.Length; i++) {
            Transform spawnpoint = Spawnpoints.Instance.GetSpawnpoint(i);
            GameObject prefab = playersRandomized[i].isCPU ? cpuVehiclePrefab : vehiclePrefab;
            Vehicle vehicle = Instantiate(prefab, spawnpoint.position, spawnpoint.rotation).GetComponent<Vehicle>();
            vehicle.Initialize(playersRandomized[i].playerIndex);
        }

        TrackManager.Instance.EndPodium.gameObject.SetActive(false);
    }

    private void Update() {
        for (int i = -1; i < Gamepad.all.Count; i++) {
            if (InputManager.GetBackPressed(i)) {
                SceneManager.LoadScene("Start");
            }
        }
    }

    private IEnumerator Countdown() {
        for (int i = 0; i < countdownTime; i++) {
            countdownText.text = (countdownTime - i).ToString();
            yield return new WaitForSeconds(1);
        }
        countdownText.gameObject.SetActive(false);
        CountdownOver = true;
    }

    private void OnPlayerFinish(int playerIndex) {
        numPlayersFinished++;
        GameInfo.GetPlayer(playerIndex).endingTime = TimeElapsed;
        if (numPlayersFinished == GameInfo.CurrentPlayers) {
            Sound.Play(raceEndSound);
            StartCoroutine(LoadEndAfterDelay());
        }
    }

    private IEnumerator LoadEndAfterDelay() {
        yield return new WaitForSeconds(raceEndDelay);
        SceneManager.LoadScene("End");
    }

}