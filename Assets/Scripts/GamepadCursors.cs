using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadCursors : MonoBehaviour {

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private RectTransform[] cursors;

    private void Awake() {
        InputSystem.onDeviceChange += OnDeviceChange;
        SetCursorVisibility();
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update() {
        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < Gamepad.all.Count; i++) {
            cursors[i].transform.position += (Vector3)(Gamepad.all[i].leftStick.value * (moveSpeed * Time.deltaTime));
            cursors[i].transform.position = new Vector3(
                Mathf.Clamp(cursors[i].transform.position.x, 0, Screen.width),
                Mathf.Clamp(cursors[i].transform.position.y, 0, Screen.height),
                0
            );

            if (Gamepad.all[i].buttonSouth.wasPressedThisFrame) {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = cursors[i].transform.position;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                Button[] buttons = results.Select(r => r.gameObject.GetComponent<Button>()).Where(b => b).ToArray();
                if (buttons.Length > 0) {
                    if (buttons[0].TryGetComponent(out PlayerButton playerButton)) {
                        playerButton.Click(i);
                    } else {
                        buttons[0].onClick.Invoke();
                    }
                }
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
    }

}