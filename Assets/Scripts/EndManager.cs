using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] winners;
    private Renderer[] winnerRenderers;

    private GameInfo.Player[] playersSorted;

    private void Awake() {
        winnerRenderers = new Renderer[winners.Length];
        for (int i = 0; i < winners.Length; i++) {
            winnerRenderers[i] = winners[i].GetComponent<Renderer>();
        }
        playersSorted = GameInfo.GetPlayers().Where(p => p != null).OrderBy(p => p.endingTime).ToArray();
        for (int i = 0; i < winners.Length; i++) {
            if (i < playersSorted.Length) {
                winnerRenderers[i].material.color = playersSorted[i].color;
            } else {
                winners[i].SetActive(false);
            }
        }
    }

    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            LoadStart();
        }
        for (int i = 0; i < Gamepad.all.Count; i++) {
            if (Gamepad.all[i].selectButton.wasPressedThisFrame) {
                LoadStart();
            }
        }
    }

    private void LoadStart() {
        SceneManager.LoadScene("Start");
    }

}