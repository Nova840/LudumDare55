using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class StartPlayers : MonoBehaviour {

    [SerializeField]
    private StartPlayer[] startPlayers;

    [SerializeField]
    private Color[] playerColors;

    [SerializeField]
    private Sound clickSound;

    private void Awake() {
        InputSystem.onDeviceChange += OnDeviceChange;
        for (int i = 0; i < startPlayers.Length; i++) {
            int buttonIndex = i;//otherwise i will be kept in memory and always be 4 in the lambda

            startPlayers[buttonIndex].AddRemoveButton.onClick.AddListener(() => OnAddRemoveButtonClick(buttonIndex, -1));
            startPlayers[buttonIndex].AddRemovePlayerButton.OnClick += controller => OnAddRemoveButtonClick(buttonIndex, controller);

            startPlayers[buttonIndex].ColorButton.onClick.AddListener(() => OnColorButtonClick(buttonIndex));

            startPlayers[buttonIndex].CPUToggle.onValueChanged.AddListener(isOn => OnCPUToggleValueChanged(buttonIndex, isOn, -1));
            startPlayers[buttonIndex].CpuPlayerToggle.OnClick += (isOn, controller) => OnCPUToggleValueChanged(buttonIndex, isOn, controller);

            startPlayers[buttonIndex].VehicleDropdown.onValueChanged.AddListener(option => OnVehicleDropdownValueChanged(buttonIndex, option));
        }
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Start() {
        RefreshPlayers();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        for (int i = 0; i < GameInfo.MaxPlayers; i++) {
            if (i >= Gamepad.all.Count) {
                GameInfo.Player player = GameInfo.GetNonCPUPlayerForController(i);
                if (player != null) {
                    GameInfo.RemovePlayer(player.playerIndex);
                }
            }
        }
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

    private void OnCPUToggleValueChanged(int startPlayerIndex, bool isOn, int controller) {
        GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
        if (player != null) {
            if (!(!isOn && GameInfo.GetNonCPUPlayerForController(controller) != null)) {
                Sound.Play(clickSound);
                player.controller = controller;
                player.isCPU = isOn;
            }
        }
        RefreshPlayers();
    }

    private void OnAddRemoveButtonClick(int startPlayerIndex, int controller) {
        Sound.Play(clickSound);
        bool playerExists = GameInfo.GetPlayer(startPlayerIndex) != null;
        if (playerExists) {
            GameInfo.RemovePlayer(startPlayerIndex);
        } else {
            bool isCPU = GameInfo.GetNonCPUPlayerForController(controller) != null;
            Color color = playerColors[Random.Range(0, playerColors.Length)];
            color = ValidPlayerColor(color);
            int vehicleIndex = Random.Range(0, startPlayers[startPlayerIndex].VehicleDropdown.options.Count);
            GameInfo.SetPlayer(new GameInfo.Player(startPlayerIndex, controller, color, isCPU, vehicleIndex));
        }
        RefreshPlayers();
    }

    private void RefreshPlayers() {
        for (int startPlayerIndex = 0; startPlayerIndex < startPlayers.Length; startPlayerIndex++) {
            GameInfo.Player player = GameInfo.GetPlayer(startPlayerIndex);
            bool playerExists = player != null;
            StartPlayer startPlayer = startPlayers[startPlayerIndex];

            startPlayer.PlayerText.gameObject.SetActive(playerExists);
            startPlayer.PlayerText.text = playerExists ? "Player: " + (player.playerIndex + 1) : "";

            startPlayer.ControllerText.gameObject.SetActive(playerExists);
            if (playerExists && !player.isCPU) {
                startPlayer.ControllerText.text = player.controller == -1 ? "Keyboard" : "Controller " + (player.controller + 1);
            } else {
                startPlayer.ControllerText.text = "";
            }

            startPlayer.AddRemoveButtonText.text = playerExists ? "Remove" : "Add";

            startPlayer.PlayerImage.color = playerExists ? player.color : Color.black;

            startPlayer.CPUToggle.gameObject.SetActive(playerExists);
            startPlayer.CPUToggle.SetIsOnWithoutNotify(playerExists ? player.isCPU : false);

            startPlayer.VehicleDropdown.gameObject.SetActive(playerExists);
            startPlayer.VehicleDropdown.SetValueWithoutNotify(playerExists ? player.vehicleIndex : 0);
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