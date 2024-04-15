using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button quitGameButton;

    [SerializeField]
    private string levelName;

    [SerializeField]
    private Sound startButtonSound;

    [SerializeField]
    private Sound quitGameButtonSound;

    private void Awake() {
        GameInfo.StartSceneLoaded = true;
        startButton.onClick.AddListener(StartButtonClicked);
        quitGameButton.onClick.AddListener(QuitGameButtonClick);
        SceneManager.LoadScene("3D_TestLevel", LoadSceneMode.Additive);
    }

    private void Start() {
        TrackManager.Instance.EndPodium.gameObject.SetActive(false);
    }

    private void StartButtonClicked() {
        Sound.Play(startButtonSound, true);
        GameInfo.LevelName = levelName;
        SceneManager.LoadScene("Game");
    }

    private void QuitGameButtonClick() {
        Sound.Play(quitGameButtonSound, true);
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}