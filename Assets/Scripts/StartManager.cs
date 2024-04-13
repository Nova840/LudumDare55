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
        GameInfo.LevelName = "TestLevel";
        SceneManager.LoadScene("Game");
    }

}