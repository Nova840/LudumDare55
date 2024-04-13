using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayerUI : MonoBehaviour {

    [SerializeField]
    private int playerIndex;

    [SerializeField]
    private TMP_Text lapsText;

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
        lapsText.text = $"Lap {newLaps}/?";
    }

}