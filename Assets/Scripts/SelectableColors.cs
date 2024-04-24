using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectableColors : MonoBehaviour {

    private ColorBlock colors;

    private Selectable selectable;
    private EventTrigger eventTrigger;

    private bool pointerInside = false;
    private float timePointerLastChanged = Mathf.NegativeInfinity;

    private bool[] gamepadsInside { get; set; } = new bool[] { false, false, false, false };
    private bool[] gamepadsInsideLastFrame = new bool[] { false, false, false, false };
    public void SetGamepadInside(int controller) => gamepadsInside[controller] = true;

    private bool mouseSelectedOnPress = false;
    private bool[] gamepadSelectedOnPress = new bool[] { false, false, false, false };

    private void Awake() {
        selectable = GetComponent<Selectable>();
        colors = selectable.colors;

        ColorBlock block = colors;
        block.fadeDuration = 0;
        selectable.colors = block;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry onPointerEnter = new EventTrigger.Entry();
        onPointerEnter.eventID = EventTriggerType.PointerEnter;
        onPointerEnter.callback.AddListener(OnPointerEnter);
        eventTrigger.triggers.Add(onPointerEnter);

        EventTrigger.Entry onPointerExit = new EventTrigger.Entry();
        onPointerExit.eventID = EventTriggerType.PointerExit;
        onPointerExit.callback.AddListener(OnPointerExit);
        eventTrigger.triggers.Add(onPointerExit);
    }

    private void LateUpdate() {
        if (pointerInside && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame)) {
            timePointerLastChanged = Time.time;
        }
        for (int i = 0; i < gamepadsInside.Length; i++) {
            if (gamepadsInside[i] != gamepadsInsideLastFrame[i]) {
                timePointerLastChanged = Time.time;
            }
        }

        if (pointerInside && Mouse.current.leftButton.wasPressedThisFrame) {
            mouseSelectedOnPress = true;
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            mouseSelectedOnPress = false;
        }
        for (int i = 0; i < gamepadSelectedOnPress.Length; i++) {
            if (i < Gamepad.all.Count) {
                if (gamepadsInside[i] && Gamepad.all[i].buttonSouth.wasPressedThisFrame) {
                    gamepadSelectedOnPress[i] = true;
                }
                if (Gamepad.all[i].buttonSouth.wasReleasedThisFrame) {
                    gamepadSelectedOnPress[i] = false;
                }
            } else {
                gamepadSelectedOnPress[i] = false;
            }
        }

        ColorBlock block = colors;
        Color c;
        if (pointerInside || gamepadsInside.Any(g => g)) {
            bool pressed = mouseSelectedOnPress && Mouse.current.leftButton.isPressed;
            for (int i = 0; i < Gamepad.all.Count; i++) {
                if (gamepadsInside[i]) {
                    pressed |= gamepadSelectedOnPress[i] && Gamepad.all[i].buttonSouth.isPressed;
                }
            }
            c = pressed ? colors.pressedColor : colors.highlightedColor;
        } else {
            c = colors.normalColor;
        }
        if (block.fadeDuration > 0) {
            c = Color.Lerp(selectable.colors.normalColor, c, (Time.time - timePointerLastChanged) / block.fadeDuration);
        }
        block.normalColor = c;
        block.highlightedColor = c;
        block.pressedColor = c;
        selectable.colors = block;

        for (int i = 0; i < gamepadsInside.Length; i++) {
            gamepadsInsideLastFrame[i] = gamepadsInside[i];
            gamepadsInside[i] = false;
        }
    }

    private void OnPointerEnter(BaseEventData eventData) {
        pointerInside = true;
        timePointerLastChanged = Time.time;
    }

    private void OnPointerExit(BaseEventData eventData) {
        pointerInside = false;
        timePointerLastChanged = Time.time;
    }

}