using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private List<AudioClip> _tracks;
    [SerializeField] private AudioClip _gameOverSound;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    public event Action<float> OnSFXVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;
    private int _currentIndex;
    private bool _stopPlaying;
    private float _currentSFXVolume;
    private float _currentMusicVolume;

    private void OnEnable() { ServicesStorage.Instance.Register(this); }

    private void Start()
    {
        _currentIndex = UnityEngine.Random.Range(0, _tracks.Count);
        _musicSource.PlayOneShot(_tracks[_currentIndex]);

        _musicVolumeSlider.onValueChanged.AddListener((value) => {
            _currentMusicVolume = value;
            _musicSource.volume = value;
            OnMusicVolumeChanged?.Invoke(value);
            Debug.Log($"Changed music volume: {value}");
        });
        _sfxVolumeSlider.onValueChanged.AddListener((value) => {
            _currentSFXVolume = value;
            OnSFXVolumeChanged?.Invoke(value);
            Debug.Log($"Changed sfx volume: {value}");
        });
    }

    public float GetCurrentSFXVolume() => _currentSFXVolume;

    private void Update() 
    {
        if (_musicSource.isPlaying || _stopPlaying) return;

        if (_currentIndex < _tracks.Count - 1) _currentIndex++;
        else _currentIndex = 0;
        _musicSource.PlayOneShot(_tracks[_currentIndex]);
    }

    public void GameOverSound() 
    {
        _stopPlaying = true;
        _musicSource.Stop();
        _musicSource.PlayOneShot(_gameOverSound);
    }

    public void LoadData(GameData data)
    {
        SetSFXVolumeManually(data.SfxVolume);
        SetMusicVolumeManually(data.MusicVolume);
    }

    public void SaveData(ref GameData data)
    {
        data.SfxVolume = _currentSFXVolume;
        data.MusicVolume = _currentMusicVolume;
    }

    private void SetSFXVolumeManually(float volume) 
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _sfxVolumeSlider.value = volume;
        _currentSFXVolume = volume;
        OnSFXVolumeChanged?.Invoke(volume);
    }

    private void SetMusicVolumeManually(float volume) 
    {
        volume = Mathf.Clamp(volume, 0, 1);
        _musicVolumeSlider.value = volume;
        _currentMusicVolume = volume;
        _musicSource.volume = volume;
        OnMusicVolumeChanged?.Invoke(volume);
    }

    public void MuteSFX() => SetSFXVolumeManually(0);

    public void MuteMusic() => SetMusicVolumeManually(0);
}
