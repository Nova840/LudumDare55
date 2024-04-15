using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartPlayers : MonoBehaviour {

    [SerializeField]
    private Color[] playerColors;

    private StartPlayer[] startPlayers;

    private void Awake() {
        startPlayers = GetComponentsInChildren<StartPlayer>();
        for (int i = 0; i < startPlayers.Length; i++) {
            int buttonIndex = i;//otherwise i will be kept in memory and always be 4 in the lambda

            startPlayers[i].AddRemoveButton.onClick.AddListener(() => OnAddRemoveButtonClick(buttonIndex, -1));
            startPlayers[i].AddRemovePlayerButton.OnClick += controller => OnAddRemoveButtonClick(buttonIndex, controller);

            startPlayers[i].ColorButton.onClick.AddListener(() => OnColorButtonClick(buttonIndex));

            startPlayers[i].CPUToggle.onValueChanged.AddListener(isOn => OnCPUToggleValueChanged(buttonIndex, isOn));
        }
        RefreshPlayers();
    }

    private void OnColorButtonClick(int startPlayerIndex) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player == null) return;
        Color playerColor = player.color;
        int colorIndex = Array.IndexOf(playerColors, playerColor);
        colorIndex++;
        colorIndex %= playerColors.Length;
        player.color = playerColors[colorIndex];
        RefreshPlayers();
    }

    private void OnCPUToggleValueChanged(int startPlayerIndex, bool isOn) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player != null) {
            player.isCPU = isOn;
        }
        RefreshPlayers();
    }

    private void OnAddRemoveButtonClick(int startPlayerIndex, int controller) {
        bool playerExists = GameInfo.GetPlayer(startPlayerIndex) != null;
        if (playerExists) {
            GameInfo.RemovePlayer(startPlayerIndex);
        } else {
            GameInfo.SetPlayer(new GameInfo.Player(startPlayerIndex, controller, playerColors[Random.Range(0, playerColors.Length)], startPlayers[startPlayerIndex].CPUToggle.isOn, 0));
        }
        RefreshPlayers();
    }

    private void RefreshPlayers() {
        for (int startPlayerIndex = 0; startPlayerIndex < startPlayers.Length; startPlayerIndex++) {
            GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
            bool playerExists = player != null;

            startPlayers[startPlayerIndex].PlayerText.text = playerExists ? "Player: " + (player.playerIndex + 1) : "";
            if (playerExists && !player.isCPU) {
                startPlayers[startPlayerIndex].ControllerText.text = player.controller == -1 ? "Keyboard" : "Controller " + (player.controller + 1);
            } else {
                startPlayers[startPlayerIndex].ControllerText.text = "";
            }
            startPlayers[startPlayerIndex].AddRemoveButtonText.text = playerExists ? "Remove" : "Add";

            startPlayers[startPlayerIndex].PlayerImage.color = playerExists ? player.color : Color.black;

            startPlayers[startPlayerIndex].CPUToggle.isOn = playerExists ? player.isCPU : false;
        }
    }

}