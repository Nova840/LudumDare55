using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private string levelName;

    private void Awake() {
        GameInfo.StartSceneLoaded = true;
        startButton.onClick.AddListener(StartButtonClicked);
        SceneManager.LoadScene("3D_TestLevel", LoadSceneMode.Additive);
    }

    private void Start() {
        TrackManager.Instance.EndPodium.gameObject.SetActive(false);
    }

    private void StartButtonClicked() {
        GameInfo.LevelName = levelName;
        SceneManager.LoadScene("Game");
    }

}