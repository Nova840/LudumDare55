using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerUI : MonoBehaviour {

    [SerializeField]
    private int playerIndex;

    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private TMP_Text lapsText;

    [SerializeField]
    private Sound lapSound;

    [SerializeField]
    private Sound finalLapSound;

    private int highestLapCount = 0;

    private void Start() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            player.OnLapsChange += OnLapsChange;
            OnLapsChange(player.Laps);//needs to be in Start
        } else {
            lapsText.gameObject.SetActive(false);
        }
    }

    private void Update() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            fillImage.fillAmount = player.Mana;
        }
    }

    private void OnDestroy() {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (player != null) {
            GameInfo.GetPlayer(playerIndex).OnLapsChange -= OnLapsChange;
        }
    }

    private void OnLapsChange(int newLaps) {
        GameInfo.Player player = GameInfo.GetPlayer(playerIndex);
        if (newLaps > highestLapCount) {
            highestLapCount = newLaps;
            if (highestLapCount < player.Laps) {
                Sound.Play(lapSound);
            } else if (highestLapCount == player.Laps) {
                Sound.Play(finalLapSound);
            }
        }
        int displayLaps = Mathf.Min(TrackManager.Instance.Laps, highestLapCount + 1);
        lapsText.text = $"Lap {displayLaps}/{TrackManager.Instance.Laps}";
    }

}