using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour {

    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            LoadStart();
        }
        for (int i = 0; i < Gamepad.all.Count; i++) {
            if (Gamepad.all[i].selectButton.wasPressedThisFrame) {
                LoadStart();
            }
        }
    }

    private void LoadStart() {
        SceneManager.LoadScene("Start");
    }

}