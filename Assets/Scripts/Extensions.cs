using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    //https://forum.unity.com/threads/randomize-array-in-c.86871/#post-561135
    public static void Shuffle<T>(this T[] array) {
        for (int i = 0; i < array.Length; i++) {
            T tmp = array[i];
            int r = Random.Range(i, array.Length);
            array[i] = array[r];
            array[r] = tmp;
        }
    }

}