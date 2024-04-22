using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartPlayers : MonoBehaviour {

    [SerializeField]
    private Color[] playerColors;

    [SerializeField]
    private Sound clickSound;

    private StartPlayer[] startPlayers;

    private void Awake() {
        startPlayers = GetComponentsInChildren<StartPlayer>();
        for (int i = 0; i < startPlayers.Length; i++) {
            int buttonIndex = i;//otherwise i will be kept in memory and always be 4 in the lambda

            startPlayers[buttonIndex].AddRemoveButton.onClick.AddListener(() => OnAddRemoveButtonClick(buttonIndex, -1));
            startPlayers[buttonIndex].AddRemovePlayerButton.OnClick += controller => OnAddRemoveButtonClick(buttonIndex, controller);

            startPlayers[buttonIndex].ColorButton.onClick.AddListener(() => OnColorButtonClick(buttonIndex));

            startPlayers[buttonIndex].CPUToggle.onValueChanged.AddListener(isOn => OnCPUToggleValueChanged(buttonIndex, isOn));

            startPlayers[buttonIndex].VehicleDropdown.onValueChanged.AddListener(option => OnVehicleDropdownValueChanged(buttonIndex, option));
        }
    }

    private void Start() {
        RefreshPlayers();
    }

    private void OnVehicleDropdownValueChanged(int startPlayerIndex, int option) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player != null) {
            Sound.Play(clickSound);
            player.vehicleIndex = option;
        }
        RefreshPlayers();
    }

    private void OnColorButtonClick(int startPlayerIndex) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player == null) return;
        Sound.Play(clickSound);
        Color playerColor = player.color;
        int colorIndex = Array.IndexOf(playerColors, playerColor);
        colorIndex++;
        colorIndex %= playerColors.Length;
        Color color = playerColors[colorIndex];
        color = ValidPlayerColor(color);
        player.color = color;
        RefreshPlayers();
    }

    private void OnCPUToggleValueChanged(int startPlayerIndex, bool isOn) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player != null) {
            Sound.Play(clickSound);
            player.isCPU = isOn;
        }
        RefreshPlayers();
    }

    private void OnAddRemoveButtonClick(int startPlayerIndex, int controller) {
        Sound.Play(clickSound);
        bool playerExists = GameInfo.GetPlayer(startPlayerIndex) != null;
        if (playerExists) {
            GameInfo.RemovePlayer(startPlayerIndex);
        } else {
            Color color = playerColors[Random.Range(0, playerColors.Length)];
            color = ValidPlayerColor(color);
            GameInfo.SetPlayer(new GameInfo.Player(startPlayerIndex, controller, color, startPlayers[startPlayerIndex].CPUToggle.isOn, 0)); startPlayers[startPlayerIndex].VehicleDropdownLabel.text = "";
        }
        RefreshPlayers();
    }

    private void RefreshPlayers() {
        for (int startPlayerIndex = 0; startPlayerIndex < startPlayers.Length; startPlayerIndex++) {
            GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
            bool playerExists = player != null;
            StartPlayer startPlayer = startPlayers[startPlayerIndex];

            startPlayer.PlayerText.text = playerExists ? "Player: " + (player.playerIndex + 1) : "";
            if (playerExists && !player.isCPU) {
                startPlayer.ControllerText.text = player.controller == -1 ? "Keyboard" : "Controller " + (player.controller + 1);
            } else {
                startPlayer.ControllerText.text = "";
            }
            startPlayer.AddRemoveButtonText.text = playerExists ? "Remove" : "Add";

            startPlayer.PlayerImage.color = playerExists ? player.color : Color.black;

            startPlayer.CPUToggle.isOn = playerExists ? player.isCPU : false;

            startPlayer.VehicleDropdown.value = playerExists ? player.vehicleIndex : 0;

            if (playerExists) {
                startPlayer.VehicleDropdownLabel.text = startPlayer.VehicleDropdown.options[startPlayer.VehicleDropdown.value].text;
            } else {
                startPlayer.VehicleDropdownLabel.text = "";
            }
        }
    }

    private Color ValidPlayerColor(Color color) {
        int colorIndex = Array.IndexOf(playerColors, color);
        for (int i = 0; i < playerColors.Length; i++) {//for loop so it can't infinite loop
            if (!GameInfo.AnyPlayerIsColor(playerColors[colorIndex])) break;
            colorIndex++;
            colorIndex %= playerColors.Length;
        }
        return playerColors[colorIndex];
    }

}