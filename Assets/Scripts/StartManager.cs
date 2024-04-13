using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

    [SerializeField]
    private Button startButton;

    private void Awake() {
        startButton.onClick.AddListener(StartButtonClicked);
    }

    private void StartButtonClicked() {
        for (int i = 0; i < GameInfo.MaxPlayers; i++) {
            GameInfo.SetPlayer(i, new GameInfo.Player(i, i - 1, Color.red));
        }
        GameInfo.LevelName = "TestLevel";
        SceneManager.LoadScene("Game");
    }

}