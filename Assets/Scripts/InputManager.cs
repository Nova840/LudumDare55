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

    public static bool GetKeyboardForward(int controller) {
        if (controller < 0) {
            return Keyboard.current.wKey.isPressed;
        } else if (controller < Gamepad.all.Count) {
            return false;
        } else {
            return false;
        }
    }

    public static bool GetKeyboardBackward(int controller) {
        if (controller < 0) {
            return Keyboard.current.sKey.isPressed;
        } else if (controller < Gamepad.all.Count) {
            return false;
        } else {
            return false;
        }
    }

    public static float GetGamepadAccelerate(int controller) {
        if (controller < 0) {
            return 0;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].rightTrigger.value;
        } else {
            return 0;
        }
    }

    public static float GetGamepadBrake(int controller) {
        if (controller < 0) {
            return 0;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].leftTrigger.value;
        } else {
            return 0;
        }
    }

}