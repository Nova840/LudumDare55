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
        private int laps = 0;
        public int Laps {
            get => laps;
            set {
                laps = value;
                OnLapsChange(laps);
            }
        }
        public event Action<int> OnLapsChange;
        public Player(int playerIndex, int controller, Color color) {
            this.playerIndex = playerIndex;
            this.controller = controller;
            this.color = color;
        }
    }

    private static Player[] players = { null, null, null, null };
    public static Player[] GetPlayers() => players.ToArray();
    public static Player GetPlayer(int index) => players[index];
    public static void SetPlayer(int index, Player player) => players[index] = player;
    public static void ForEachPlayer(Action<Player> action) {
        for (int i = 0; i < players.Length; i++) {
            action?.Invoke(players[i]);
        }
    }
    public static int MaxPlayers => players.Length;
    public static int CurrentPlayers => players.Count(p => p != null);

    public static string LevelName { get; set; }

}