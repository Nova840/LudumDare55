using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject vehiclePrefab;

    private void Awake() {
        SceneManager.LoadScene(GameInfo.LevelName, LoadSceneMode.Additive);
    }

    private void Start() {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameInfo.LevelName));//not active in awake

        GameInfo.Player[] playersRandomized = GameInfo.GetPlayers().Where(p => p != null).ToArray();
        playersRandomized.Shuffle();
        for (int i = 0; i < playersRandomized.Length; i++) {
            Transform spawnpoint = Spawnpoints.Instance.GetSpawnpoint(i);
            Vehicle vehicle = Instantiate(vehiclePrefab, spawnpoint.position, spawnpoint.rotation).GetComponent<Vehicle>();
            vehicle.Initialize(playersRandomized[i].playerIndex);
        }
    }

}