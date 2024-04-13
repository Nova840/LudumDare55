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
            OnLapsChange(player.Laps);
        } else {
            lapsText.gameObject.SetActive(false);
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
        lapsText.text = $"Lap {highestLapCount}/?";
    }

}