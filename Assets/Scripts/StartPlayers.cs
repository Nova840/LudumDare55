using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartPlayers : MonoBehaviour {

    [SerializeField]
    private GameObject[] startPlayers;

    [SerializeField]
    private TMP_Text[] playerTexts;

    [SerializeField]
    private TMP_Text[] controllerTexts;

    [SerializeField]
    private Button[] addRemoveButtons;

    [SerializeField]
    private PlayerButton[] addRemovePlayerButtons;

    [SerializeField]
    private TMP_Text[] addRemoveButtonTexts;

    private void Awake() {
        for (int i = 0; i < startPlayers.Length; i++) {
            int buttonIndex = i;//otherwise i will be kept in memory and always be 4 in the lambda
            addRemoveButtons[i].onClick.AddListener(() => OnAddRemoveButtonClick(buttonIndex, -1));
        }
        RefreshPlayers();
    }

    public void OnAddRemoveButtonClick(int startPlayerIndex, int controller) {
        bool playerExists = GameInfo.GetPlayer(startPlayerIndex) != null;
        if (playerExists) {
            GameInfo.RemovePlayer(startPlayerIndex);
        } else {
            GameInfo.SetPlayer(new GameInfo.Player(startPlayerIndex, controller, Color.red));
        }
        RefreshPlayers();
    }

    private void RefreshPlayers() {
        for (int startPlayerIndex = 0; startPlayerIndex < startPlayers.Length; startPlayerIndex++) {
            GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
            bool playerExists = player != null;
            playerTexts[startPlayerIndex].text = playerExists ? "Player: " + (player.playerIndex + 1) : "";
            if (playerExists) {
                controllerTexts[startPlayerIndex].text = player.controller == -1 ? "Keyboard" : "Controller " + (player.controller + 1);
            } else {
                controllerTexts[startPlayerIndex].text = "";
            }
            addRemoveButtonTexts[startPlayerIndex].text = playerExists ? "Remove" : "Add";
        }
    }

}