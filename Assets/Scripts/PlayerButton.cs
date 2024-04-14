using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : MonoBehaviour {

    [SerializeField]
    private int buttonIndex;

    public event Action<int> OnClick;//<controller>

    public void Click(int controller) {
        OnClick?.Invoke(controller);
    }

}