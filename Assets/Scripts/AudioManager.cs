using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private List<AudioClip> _tracks;
    [SerializeField] private AudioClip _buttonClickedSound;
    [SerializeField] private AudioClip _gameOverSound;
    private int _currentIndex;
    private bool _stopPlaying;

    private void Start()
    {
        ServicesStorage.Instance.Register(this);
        _currentIndex = UnityEngine.Random.Range(0,2);
        Debug.Log(_currentIndex);
        _musicSource.PlayOneShot(_tracks[_currentIndex]);
    }
    private void Update() 
    {
        if (_musicSource.isPlaying || _stopPlaying) return;

        if (_currentIndex < _tracks.Count - 1) _currentIndex++;
        else _currentIndex = 0;
        _musicSource.PlayOneShot(_tracks[_currentIndex]);
    }

    public void PlayOneShot(AudioClip clip, float volume = 1)
    {
        _sfxSource.pitch = 1;
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip, float pitch, float volume = 1)
    {
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip, float pitchMin, float pitchMax, float volume = 1)
    {
        _sfxSource.pitch = Random.Range(pitchMin,pitchMax);
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
    }

    public void PressButtonSound() 
    {
        PlayOneShot(_buttonClickedSound);
    }

    public void GameOverSound() 
    {
        _stopPlaying = true;
        _musicSource.Stop();
        _musicSource.PlayOneShot(_gameOverSound);
    }
}
