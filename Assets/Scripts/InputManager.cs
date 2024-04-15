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

    public static float GetAccelerate(int controller) {
        if (controller < 0) {
            return Keyboard.current.wKey.value;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].rightTrigger.value;
        } else {
            return 0;
        }
    }

    public static float GetReverse(int controller) {
        if (controller < 0) {
            return Keyboard.current.sKey.value;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].leftTrigger.value;
        } else {
            return 0;
        }
    }

    public static float GetBrake(int controller) {
        if (controller < 0) {
            return Keyboard.current.leftShiftKey.value;
        } else if (controller < Gamepad.all.Count) {
            return Mathf.Clamp01(Gamepad.all[controller].leftShoulder.value + Gamepad.all[controller].rightShoulder.value);
        } else {
            return 0;
        }
    }

    public static bool GetBackPressed(int controller) {
        if (controller < 0) {
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].selectButton.wasPressedThisFrame;
        } else {
            return false;
        }
    }

}