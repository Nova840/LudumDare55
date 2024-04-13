using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayerUI : MonoBehaviour {

    [SerializeField]
    private int playerIndex;

    [SerializeField]
    private TMP_Text lapsText;

    private int highestLapCount = 0;

    private void Awake() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            player.OnLapsChange += OnLapsChange;
        } else {
            lapsText.gameObject.SetActive(false);
        }
    }

    private void Start() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            OnLapsChange(player.Laps);
        }
    }

    private void OnDestroy() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            GameInfo.GetPlayer(playerIndex).OnLapsChange -= OnLapsChange;
        }
    }

    private void OnLapsChange(int newLaps) {
        if (newLaps > highestLapCount) {
            highestLapCount = newLaps;
        }
        int displayLaps = Mathf.Min(TrackManager.Instance.Laps, highestLapCount + 1);
        lapsText.text = $"Lap {displayLaps}/{TrackManager.Instance.Laps}";
    }

}