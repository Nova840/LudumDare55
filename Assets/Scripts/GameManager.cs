using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
    }

    private void OnDestroy() {
        GameInfo.OnPlayerFinish -= OnPlayerFinish;
    }

    private void Start() {
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
            SceneManager.LoadScene("End");
        }
    }

}