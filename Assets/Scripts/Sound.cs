﻿using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public AudioClip clip;
    public AudioMixerGroup mixer;
    public string name;
    public bool loop;
    public bool ignorePause;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

}