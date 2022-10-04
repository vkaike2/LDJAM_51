using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> _audios;
    [SerializeField]
    private AudioSource _audioLoopSource;

    private AudioSource _audioSource;

    private float _initialVolume;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _initialVolume = _audioSource.volume;
    }

    public void PlayAudio(string name)
    {
        _audioSource.loop = false;
       
        if (Configuration.IsMuted)
        {
            return;
        }
        AudioClip audio = _audios.FirstOrDefault(e => e.name == name);
        _audioSource.clip = audio;
        _audioSource.Play();
    }

    internal void Unmute()
    {
        _audioSource.volume = _initialVolume;
    }

    internal void Mute()
    {
        _audioSource.volume = 0;
    }

    internal void PlayLoopAudio(string v)
    {
        print(v);
        _audioSource.loop = true;
        _audioSource.clip = _audios.FirstOrDefault(e => e.name == v);
        _audioSource.Play();
    }
}
