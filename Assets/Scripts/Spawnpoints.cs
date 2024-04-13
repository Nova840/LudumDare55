using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoints : MonoBehaviour {

    public static Spawnpoints Instance { get; private set; }

    private Transform[] points;
    public Transform GetSpawnpoint(int index) => points[index];

    private void Awake() {
        Instance = this;
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.GetChild(i);
        }
    }

}