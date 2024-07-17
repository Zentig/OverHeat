using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BulletSound : MonoBehaviour
{
    public event Action<BulletSound> OnDestroyed;
    [SerializeField] private float _lifetime = 4f;
    private AudioSource _audioSrc;
    private bool _isGamePaused;
    private float _timePassed = 0;

    public void Init()
    {
        _audioSrc = GetComponent<AudioSource>();
    }

    public void ResetProperties() => _timePassed = 0;

    public void HandlePauseState(bool state) => _isGamePaused = state;

    private void Update() 
    {
        if (_isGamePaused) { return; }
        
        _timePassed += Time.deltaTime;
        if (_timePassed >= _lifetime) 
        {
            ResetProperties();
            OnDestroyed?.Invoke(this);
        } 
    }

    public void PlayShootSound(float volume, AudioClip sound, float minPitch, float maxPitch) 
    {
        _audioSrc.volume = volume;
        _audioSrc.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        _audioSrc.PlayOneShot(sound);
    }
}