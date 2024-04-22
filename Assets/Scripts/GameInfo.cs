using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameInfo {

    public class Player {
        public int playerIndex;
        public int controller;
        public Color color;
        private int laps = -1;
        public int Laps => laps;
        public void AddLap() {
            laps++;
            lastLapChangeForward = true;
            OnLapsChange?.Invoke(laps);
            if (HasFinished) {
                OnPlayerFinish?.Invoke(playerIndex);
            }
        }
        public void SubtractLap() {
            if (!lastLapChangeForward) return;
            laps--;
            lastLapChangeForward = false;
            OnLapsChange?.Invoke(laps);
        }
        public void Reset() {
            laps = -1;
            lastLapChangeForward = false;
            OnLapsChange?.Invoke(laps);
            endingTime = 0;
            Mana = 0;
        }
        public event Action<int> OnLapsChange;
        public bool lastLapChangeForward = false;
        public bool HasFinished => laps >= TrackManager.Instance.Laps;
        public float endingTime;
        public bool isCPU;
        public int vehicleIndex;
        public float Mana { get; set; }
        public Player(int playerIndex, int controller, Color color, bool isCPU, int vehicleIndex) {
            this.playerIndex = playerIndex;
            this.controller = controller;
            this.color = color;
            this.isCPU = isCPU;
            this.vehicleIndex = vehicleIndex;
        }
    }

    private static Player[] players = { null, null, null, null };
    public static Player[] GetPlayers() => players.ToArray();
    public static Player GetPlayer(int playerIndex) => players[playerIndex];
    public static void SetPlayer(Player player) {
        players[player.playerIndex] = player;
        OnPlayersChange?.Invoke();
    }
    public static void RemovePlayer(int playerIndex) {
        players[playerIndex] = null;
        OnPlayersChange?.Invoke();
    }
    public static void ForEachPlayer(Action<Player> action) {
        for (int i = 0; i < players.Length; i++) {
            action?.Invoke(players[i]);
        }
    }
    public static bool AnyPlayerIsColor(Color color) => players.Any(p => p != null && p.color == color);
    public static bool AllPlayersFinished() => players.All(p => p == null || p.HasFinished);
    public static Player GetNonCPUPlayerForController(int controller) => players.FirstOrDefault(p => p != null && !p.isCPU && p.controller == controller);
    public static int MaxPlayers => players.Length;
    public static int CurrentPlayers => players.Count(p => p != null);
    public static event Action<int> OnPlayerFinish;
    public static event Action OnPlayersChange;

    public static string LevelName { get; set; }

    public static bool StartSceneLoaded { get; set; }
    public static bool EndSceneLoaded { get; set; }

}