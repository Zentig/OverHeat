using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionParticlesExtra : MonoBehaviour
{
    [SerializeField] private AudioClip _explosionSound;
    private GameOverMenu _gameOverMenu;
    private AudioSource _audioSource;
    private AudioManager _audioManager;

    void Start()
    {
        _gameOverMenu = ServicesStorage.Instance.Get<GameOverMenu>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
    }

    public void PlayExplosionSound() 
    {
        _audioSource.volume = _audioManager.GetCurrentSFXVolume();
        _audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.3f);
        _audioSource.PlayOneShot(_explosionSound);
    }

    public void PlayGameOver() => _gameOverMenu.PlayGameOver();
}
