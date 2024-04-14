using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour {

    public static TrackManager Instance { get; private set; }

    [SerializeField]
    private int laps;
    public int Laps => laps;

    private void Awake() {
        Instance = this;
        if (!GameInfo.StartSceneLoaded) {
            GameInfo.SetPlayer(new GameInfo.Player(0, -1, Color.red, true));
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
    }

}