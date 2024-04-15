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
    private bool testCPU;

    private enum TestControls { Keyboard = -1, Controller1 = 0, Controller2 = 1, Controller3 = 2, Controller4 = 3 }
    [SerializeField]
    private TestControls testControls;

    [SerializeField]
    private Color testColor;

    private enum TestVehicle { ShoppingCart = 0, Suitcase = 1, Wheelchair = 2, Hoverboard = 3 }
    [SerializeField]
    private TestVehicle testVehicle;

    [SerializeField]
    private Camera startCamera;
    public Camera StartCamera => startCamera;

    [SerializeField]
    private Camera endCamera;
    public Camera EndCamera => endCamera;

    [SerializeField]
    private Podium endPodium;
    public Podium EndPodium => endPodium;

    private void Awake() {
        Instance = this;
        if (!GameInfo.StartSceneLoaded && !GameInfo.EndSceneLoaded) {
            GameInfo.LevelName = SceneManager.GetActiveScene().name;
            GameInfo.SetPlayer(new GameInfo.Player(0, (int)testControls, testColor, testCPU, (int)testVehicle));
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
    }

}