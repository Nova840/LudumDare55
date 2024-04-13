using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager {

    public static Vector2 GetMoveVector(int controller) {
        if (controller < 0) {
            return new Vector2(Keyboard.current.dKey.value - Keyboard.current.aKey.value, Keyboard.current.wKey.value - Keyboard.current.sKey.value);
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].leftStick.value;
        } else {
            return Vector2.zero;
        }
    }

}