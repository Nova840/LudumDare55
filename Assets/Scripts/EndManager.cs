using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour {

    [SerializeField]
    private GameObject vehiclePrefab;

    [SerializeField]
    private Sound goBackSound;

    private GameInfo.Player[] playersSorted;

    private void Awake() {
        GameInfo.EndSceneLoaded = true;
        SceneManager.LoadScene(GameInfo.LevelName, LoadSceneMode.Additive);
    }

    private void Start() {
        Transform[] winnersSpawnpoints = TrackManager.Instance.EndPodium.WinnersSpawnpoints;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameInfo.LevelName));
        playersSorted = GameInfo.GetPlayers().Where(p => p != null).OrderBy(p => p.endingTime).ToArray();
        for (int i = 0; i < playersSorted.Length; i++) {
            Transform spawnpoint = winnersSpawnpoints[i];
            Vehicle vehicle = Instantiate(vehiclePrefab, spawnpoint.position, spawnpoint.rotation).GetComponent<Vehicle>();
            vehicle.Initialize(playersSorted[i].playerIndex);
            vehicle.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void Update() {
        for (int i = -1; i < Gamepad.all.Count; i++) {
            if (InputManager.GetBackPressed(i)) {
                Sound.Play(goBackSound, true);
                SceneManager.LoadScene("Start");
                break;
            }
        }
    }

}