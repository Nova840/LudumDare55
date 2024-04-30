using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager {

    public static Vector2 GetMoveVector(int controller) {
        if (controller < 0) {
            return new Vector2(Keyboard.current.dKey.value - Keyboard.current.aKey.value, Keyboard.current.wKey.value - Keyboard.current.sKey.value);
        } else if (controller < Gamepad.all.Count) {
            return Vector2.ClampMagnitude(Gamepad.all[controller].leftStick.value + Gamepad.all[controller].dpad.value, 1);
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

    public static float GetRespawn(int controller) {
        if (controller < 0) {
            return Keyboard.current.rKey.value;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].buttonNorth.value;
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

    public static bool GetSummonSpeedBoost(int controller) {
        if (controller < 0) {
            return Keyboard.current.leftArrowKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].buttonWest.wasPressedThisFrame;
        } else {
            return false;
        }
    }

    public static bool GetSummonExploding(int controller) {
        if (controller < 0) {
            return Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].buttonEast.wasPressedThisFrame;
        } else {
            return false;
        }
    }

    public static bool GetSummonBouncePad(int controller) {
        if (controller < 0) {
            return Keyboard.current.upArrowKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;
        } else if (controller < Gamepad.all.Count) {
            return Gamepad.all[controller].buttonSouth.wasPressedThisFrame;
        } else {
            return false;
        }
    }

}