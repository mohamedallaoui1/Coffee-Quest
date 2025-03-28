﻿using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.ignoreListenerPause = s.ignorePause;
        }
    }

    private void Start()
    {
		SetMusic(true);
    }

    public void Play(string name)
    {
        //Sound s = Array.Find(sounds, sound => sound.clip.name.ToLower() == name.ToLower());
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) // debugging
        {
            Debug.LogWarning("Sound:" + name + " missing.");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Stop();
    }

    public void StopAllSounds()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.Stop();
        }
    }

    public void SetMusic(bool onOrOff)
    {
        if (onOrOff)
        {
            AudioManager.instance.Play("Music");
        }
        else
        {
            AudioManager.instance.Stop("Music");
        }
    }
}
