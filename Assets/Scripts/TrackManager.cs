using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class TrackManager : MonoBehaviour {

    public static TrackManager Instance { get; private set; }

    [SerializeField]
    private int laps;
    public int Laps => laps;

    [SerializeField]
    private PathCreator cpuPath;
    public PathCreator CPUPath => cpuPath;

    [SerializeField]
    private bool cpuPathReverse;
    public bool CPUPathReverse => cpuPathReverse;

    [SerializeField, Range(0, 1)]
    private float cpuPathStartPercent;
    public float CPUPathStartPercent => cpuPathStartPercent;

    [SerializeField]
    private float cpuPathVerticalOffset;
    public float CPUPathVerticalOffset => cpuPathVerticalOffset;

    [SerializeField]
    private Camera podiumCamera;
    public Camera PodiumCamera => podiumCamera;

    [SerializeField]
    private Podium endPodium;
    public Podium EndPodium => endPodium;

    [Header("Only Required If Start")]
    [SerializeField]
    private Camera startCamera;
    public Camera StartCamera => startCamera;

    [Header("Testing")]
    [SerializeField]
    private TestPlayer[] testPlayers;

    [Serializable]
    private class TestPlayer {
        public bool isCPU;

        public Controls controls;
        public enum Controls { Keyboard = -1, Controller1 = 0, Controller2 = 1, Controller3 = 2, Controller4 = 3 }

        public Color color;

        public Vehicle vehicle;
        public enum Vehicle { ShoppingCart = 0, Suitcase = 1, Wheelchair = 2, Hoverboard = 3 }
    }

    [Header("Old Crap")]
    [SerializeField]
    private SplineContainer oldCPUPath;
    public SplineContainer OldCPUPath => oldCPUPath;

    private void Awake() {
        Instance = this;
        if (!GameInfo.StartSceneLoaded && !GameInfo.EndSceneLoaded) {
            GameInfo.LevelName = SceneManager.GetActiveScene().name;
            for (int i = 0; i < testPlayers.Length; i++) {
                GameInfo.SetPlayer(new GameInfo.Player(i, (int)testPlayers[i].controls, testPlayers[i].color, testPlayers[i].isCPU, (int)testPlayers[i].vehicle));
            }
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
    }

    private void OnDrawGizmosSelected() {
        if (!cpuPath && !oldCPUPath) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(FindAnyObjectByType<TrackManager>().GetPathPoint(cpuPathStartPercent) + Vector3.up * cpuPathVerticalOffset, 1);
    }

    public float GetPathLength() {
        if (!cpuPath) {
            return oldCPUPath.CalculateLength();
        } else {
            return cpuPath.path.length;
        }
    }

    public Vector3 GetPathPoint(float time) {
        if (!cpuPath) {
            return oldCPUPath.EvaluatePosition(time);
        } else {
            return cpuPath.path.GetPointAtTime(time);
        }
    }

}