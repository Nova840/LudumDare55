using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {

    public class Player {
        public int controller;
        public Color color;
        public Player(int controller, Color color) {
            this.controller = controller;
            this.color = color;
        }
    }

    private static Player[] players = { null, null, null, null };
    public static void SetPlayer(int index, Player player) {
        players[index] = player;
    }
    public static int MaxPlayers => players.Length;

}