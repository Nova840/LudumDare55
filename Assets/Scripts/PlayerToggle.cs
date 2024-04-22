using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToggle : MonoBehaviour {

    public event Action<bool, int> OnClick;//<isOn, controller>

    public void Click(bool isOn, int controller) {
        OnClick?.Invoke(isOn, controller);
    }

}