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
        }
        public event Action<int> OnLapsChange;
        public bool lastLapChangeForward = false;
        public bool HasFinished => laps >= TrackManager.Instance.Laps;
        public float endingTime;
        public bool isCPU;
        public Player(int playerIndex, int controller, Color color, bool isCPU) {
            this.playerIndex = playerIndex;
            this.controller = controller;
            this.color = color;
            this.isCPU = isCPU;
        }
    }

    private static Player[] players = { null, null, null, null };
    public static Player[] GetPlayers() => players.ToArray();
    public static Player GetPlayer(int playerIndex) => players[playerIndex];
    public static void SetPlayer(Player player) => players[player.playerIndex] = player;
    public static void RemovePlayer(int playerIndex) => players[playerIndex] = null;
    public static void ForEachPlayer(Action<Player> action) {
        for (int i = 0; i < players.Length; i++) {
            action?.Invoke(players[i]);
        }
    }
    public static int MaxPlayers => players.Length;
    public static int CurrentPlayers => players.Count(p => p != null);
    public static event Action<int> OnPlayerFinish;

    public static string LevelName { get; set; }

    public static bool StartSceneLoaded { get; set; }

}