using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour {

    [SerializeField]
    private int laps;
    public int Laps => laps;

    private void Awake() {
        if (!GameInfo.StartSceneLoaded) {
            GameInfo.SetPlayer(new GameInfo.Player(0, -1, Color.red));
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
    }

}