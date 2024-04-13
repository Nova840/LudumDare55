using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : MonoBehaviour {

    [SerializeField]
    private int buttonIndex;

    private StartPlayers startPlayers;

    private void Awake() {
        startPlayers = GetComponentInParent<StartPlayers>(true);
    }

    public void Click(int controller) {
        startPlayers.OnAddRemoveButtonClick(buttonIndex, controller);
    }

}