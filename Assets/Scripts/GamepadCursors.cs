using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadCursors : MonoBehaviour {

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float moveAcceleration;

    [SerializeField]
    private RectTransform[] cursors;

    private Selectable[] controllerSelectedOnPress = new Selectable[4];

    private Vector2[] cursorCurrentVelocities = new Vector2[4];

    private void Awake() {
        InputSystem.onDeviceChange += OnDeviceChange;
        SetCursorVisibility();
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update() {
        EventSystem.current.SetSelectedGameObject(null);
        for (int controller = 0; controller < Gamepad.all.Count; controller++) {
            Vector2 inputVector = Vector2.ClampMagnitude(Gamepad.all[controller].leftStick.value + Gamepad.all[controller].dpad.value, 1);
            Vector2 targetVelocity = (Vector3)(inputVector * (moveSpeed * Time.deltaTime));
            if (targetVelocity == Vector2.zero) {
                cursorCurrentVelocities[controller] = Vector2.zero;
            } else {
                cursorCurrentVelocities[controller] = Vector2.MoveTowards(cursorCurrentVelocities[controller], targetVelocity, moveAcceleration * Time.deltaTime);
            }
            cursors[controller].transform.position += (Vector3)cursorCurrentVelocities[controller];
            cursors[controller].transform.position = new Vector3(
                Mathf.Clamp(cursors[controller].transform.position.x, 0, Screen.width),
                Mathf.Clamp(cursors[controller].transform.position.y, 0, Screen.height),
                0
            );

            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = cursors[controller].transform.position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            Selectable[] selectables = results.Select(r => r.gameObject.GetComponentInParent<Selectable>()).Where(b => b && b.interactable).ToArray();
            if (selectables.Length > 0) {
                Selectable selected = selectables[0];
                SelectableColors selectableColors = selected.GetComponent<SelectableColors>();
                if (selectableColors) {
                    selectableColors.SetGamepadInside(controller);
                }
                if (Gamepad.all[controller].buttonSouth.wasPressedThisFrame) {
                    controllerSelectedOnPress[controller] = selected;
                } else if (Gamepad.all[controller].buttonSouth.wasReleasedThisFrame && selected == controllerSelectedOnPress[controller]) {
                    if (selected is Button button) {
                        if (button.TryGetComponent(out PlayerButton playerButton)) {
                            playerButton.Click(controller);
                        } else {
                            button.onClick.Invoke();
                        }
                    } else if (selected is Toggle toggle) {
                        if (toggle.TryGetComponent(out PlayerToggle playerToggle)) {
                            playerToggle.Click(!toggle.isOn, controller);
                        } else {
                            toggle.isOn = !toggle.isOn;
                        }
                    } else if (selected is TMP_Dropdown dropdown) {
                        int value = dropdown.value;
                        value++;
                        value %= dropdown.options.Count;
                        dropdown.value = value;
                    }
                }
            }
            if (Gamepad.all[controller].buttonSouth.wasReleasedThisFrame) {
                controllerSelectedOnPress[controller] = null;
            }
        }
    }

    private void SetCursorVisibility() {
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].gameObject.SetActive(i < Gamepad.all.Count);
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        SetCursorVisibility();
        for (int i = 0; i < GameInfo.MaxPlayers; i++) {
            if (i >= Gamepad.all.Count) {
                cursorCurrentVelocities[i] = Vector2.zero;
            }
        }
    }

}