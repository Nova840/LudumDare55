using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour {

    private GameInfo.Player[] playersSorted;

    private void Awake() {
        GameInfo.EndSceneLoaded = true;
        SceneManager.LoadScene(GameInfo.LevelName, LoadSceneMode.Additive);
    }

    private void Start() {
        Renderer[] winnerRenderers = TrackManager.Instance.EndPodium.WinnerRenderers;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameInfo.LevelName));
        playersSorted = GameInfo.GetPlayers().Where(p => p != null).OrderBy(p => p.endingTime).ToArray();
        for (int i = 0; i < winnerRenderers.Length; i++) {
            if (i < playersSorted.Length) {
                winnerRenderers[i].material.color = playersSorted[i].color;
            } else {
                winnerRenderers[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        for (int i = -1; i < Gamepad.all.Count; i++) {
            if (InputManager.GetBackPressed(i)) {
                SceneManager.LoadScene("Start");
            }
        }
    }

}