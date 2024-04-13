using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject vehiclePrefab;

    private float timeStarted;
    public float TimeElapsed => Time.time - timeStarted;

    private int numPlayersFinished = 0;

    private void Awake() {
        Instance = this;
        if (GameInfo.StartSceneLoaded) {
            SceneManager.LoadScene(GameInfo.LevelName, LoadSceneMode.Additive);
        }
        GameInfo.ForEachPlayer(p => {
            p?.Reset();
        });
        GameInfo.OnPlayerFinish += OnPlayerFinish;
    }

    private void OnDestroy() {
        GameInfo.OnPlayerFinish -= OnPlayerFinish;
    }

    private void Start() {
        if (GameInfo.StartSceneLoaded) {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameInfo.LevelName));//not active in awake
        }

        timeStarted = Time.time;

        GameInfo.Player[] playersRandomized = GameInfo.GetPlayers().Where(p => p != null).ToArray();
        playersRandomized.Shuffle();
        for (int i = 0; i < playersRandomized.Length; i++) {
            Transform spawnpoint = Spawnpoints.Instance.GetSpawnpoint(i);
            Vehicle vehicle = Instantiate(vehiclePrefab, spawnpoint.position, spawnpoint.rotation).GetComponent<Vehicle>();
            vehicle.Initialize(playersRandomized[i].playerIndex);
        }
    }

    private void OnPlayerFinish(int playerIndex) {
        numPlayersFinished++;
        GameInfo.GetPlayer(playerIndex).endingTime = TimeElapsed;
        if (numPlayersFinished == GameInfo.CurrentPlayers) {
            SceneManager.LoadScene("End");
        }
    }

}