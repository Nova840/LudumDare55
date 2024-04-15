using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podium : MonoBehaviour {

    [SerializeField]
    private GameObject[] winners;
    private Renderer[] winnerRenderers;
    public Renderer[] WinnerRenderers => winnerRenderers;

    private void Awake() {
        winnerRenderers = new Renderer[winners.Length];
        for (int i = 0; i < winners.Length; i++) {
            winnerRenderers[i] = winners[i].GetComponent<Renderer>();
        }
    }

}