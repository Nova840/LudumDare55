using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[IncludeInSettings(true)]
[Serializable]
public class Sound {

    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 1;

    [Range(0, 2)]
    public float randomPitchVariation = 0;

    public Sound(AudioClip clip, float volume = 1, float randomPitchVariation = 0) {
        this.clip = clip;
        this.volume = volume;
        this.randomPitchVariation = randomPitchVariation;
    }

    public static void Play(Sound[] sounds, bool dontDestroyOnLoad = false) => Play(sounds[Random.Range(0, sounds.Length)], dontDestroyOnLoad);
    public static void Play(Sound sound, bool dontDestroyOnLoad = false) => Play(sound.clip, sound.volume, sound.randomPitchVariation, dontDestroyOnLoad);
    public static void Play(AudioClip clip, float volume = 1, float randomPitchVariation = 0, bool dontDestroyOnLoad = false) {
        AudioSource source = new GameObject("Sound: " + clip.name).AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(1 / (randomPitchVariation + 1), randomPitchVariation + 1);
        source.Play();
        if (dontDestroyOnLoad) {
            Object.DontDestroyOnLoad(source.gameObject);
        }
        Object.Destroy(source.gameObject, clip.length / source.pitch);
    }

}