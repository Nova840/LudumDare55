using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {

    [SerializeField]
    private Button level1Button;

    [SerializeField]
    private Button level2Button;

    [SerializeField]
    private Button quitGameButton;

    [SerializeField]
    private Sound startButtonSound;

    [SerializeField]
    private Sound quitGameButtonSound;

    private void Awake() {
        GameInfo.StartSceneLoaded = true;
        level1Button.onClick.AddListener(Level1ButtonClicked);
        level2Button.onClick.AddListener(Level2ButtonClicked);
        quitGameButton.onClick.AddListener(QuitGameButtonClick);
        SceneManager.LoadScene("3D_TestLevel", LoadSceneMode.Additive);
    }

    private void Start() {
        TrackManager.Instance.EndPodium.gameObject.SetActive(false);
    }

    private void Level1ButtonClicked() {
        Sound.Play(startButtonSound, true);
        GameInfo.LevelName = "3D_TestLevel";
        SceneManager.LoadScene("Game");
    }

    private void Level2ButtonClicked() {
        Sound.Play(startButtonSound, true);
        GameInfo.LevelName = "Level_02";
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