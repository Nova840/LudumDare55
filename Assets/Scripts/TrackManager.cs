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
    private SplineContainer cpuSpline;
    public SplineContainer CPUSpline => cpuSpline;

    [SerializeField]
    private bool cpuSplineReverse;
    public bool CPUSplineReverse => cpuSplineReverse;

    [SerializeField, Range(0, 1)]
    private float cpuSplineStartPercent;
    public float CPUSplineStartPercent => cpuSplineStartPercent;

    [SerializeField]
    private float cpuSplineVerticalOffset;
    public float CPUSplineVerticalOffset => cpuSplineVerticalOffset;

    [SerializeField]
    private bool testWithCPU;

    private void Awake() {
        Instance = this;
        if (!GameInfo.StartSceneLoaded) {
            GameInfo.SetPlayer(new GameInfo.Player(0, -1, Color.red, testWithCPU));
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
    }

}