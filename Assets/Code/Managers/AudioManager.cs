using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> _audios;
    [SerializeField]
    private AudioSource _audioLoopSource;

    private AudioSource _audioSource;

    private float _initialVolume;
    private float _initialLoopVolume;
    private SoundSettingsUI _soundSettings;

    private const float REDUCING_VOLUME_TO = 0.2f;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _initialVolume = _audioSource.volume;

        if (_audioLoopSource != null)
        {
            string nameTest = this.gameObject.name;
            _initialLoopVolume = _audioLoopSource.volume;
        }
    }

    private void Start()
    {
        _soundSettings = GameObject.FindObjectOfType<SoundSettingsUI>();
        _soundSettings.AddAudioManager(this);

        if (Configuration.IsMuted)
        {
            if (_audioLoopSource != null)
            {
                _audioLoopSource.volume = 0;
            }
            _audioSource.volume = 0;
        }
        else
        {
            if (_audioLoopSource != null)
            {
                _audioLoopSource.volume = _initialLoopVolume * REDUCING_VOLUME_TO;
            }
            _audioSource.volume = _initialVolume * REDUCING_VOLUME_TO;
        }
    }

    public void PlayAudio(string name)
    {
        _audioSource.volume = _initialVolume * REDUCING_VOLUME_TO;

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
        _audioSource.volume = _initialVolume * REDUCING_VOLUME_TO;
        if (_audioLoopSource != null)
        {
            _audioLoopSource.volume = _initialLoopVolume * REDUCING_VOLUME_TO;
        }
    }

    internal void Mute()
    {
        _audioSource.volume = 0;
        if (_audioLoopSource != null)
        {
            _audioLoopSource.volume = 0;
        }
    }

    internal void PlayLoopAudio(string v)
    {
        print(v);
        _audioSource.loop = true;
        _audioSource.clip = _audios.FirstOrDefault(e => e.name == v);
        _audioSource.Play();
    }
}
