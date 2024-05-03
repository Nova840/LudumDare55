using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

    [Serializable]
    private class LevelButton {
        public Button button;
        public string levelName;
    }

    [SerializeField]
    private LevelButton[] levelButtons;

    [SerializeField]
    private Button quitGameButton;

    [SerializeField]
    private Sound startButtonSound;

    [SerializeField]
    private Sound quitGameButtonSound;

    [SerializeField]
    private string backgroundLevelName;

    private void Awake() {
        GameInfo.StartSceneLoaded = true;
        foreach (LevelButton levelButton in levelButtons) {
            levelButton.button.onClick.AddListener(() => LevelButtonClicked(levelButton.levelName));
        }
        quitGameButton.onClick.AddListener(QuitGameButtonClick);
        SceneManager.LoadScene(backgroundLevelName, LoadSceneMode.Additive);
        GameInfo.OnPlayersChange += OnPlayersChange;
        OnPlayersChange();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDestroy() {
        GameInfo.OnPlayersChange -= OnPlayersChange;
    }

    private void Start() {
        TrackManager.Instance.EndPodium.gameObject.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(backgroundLevelName));
    }

    private void OnPlayersChange() {
        foreach (LevelButton levelButton in levelButtons) {
            levelButton.button.interactable = GameInfo.CurrentPlayers > 0;
        }
    }

    private void LevelButtonClicked(string levelName) {
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