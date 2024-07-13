using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private float _lifetime = 4f;
    public event Action<Bullet> OnDestroyed;
    private AudioSource _audioSource;
    private Rigidbody2D _rb;
    private float _timePassed = 0;
    private bool _isDestroyed = false;
    private Vector2 _frozenVelocity;
    private float _frozenGravityScale;
    private bool _isGamePaused;
    private AudioManager _audioManager;

    public void Init() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
    }

    public void OnShoot() 
    {
        _audioSource.volume = _audioManager.GetCurrentSFXVolume();
        AddForceToRigidbody();
        _isDestroyed = false;
        _timePassed = 0;
    }

    public void AddForceToRigidbody() 
    {   
        _rb.velocity = transform.right * _bulletSpeed;
    }

    private void Update() 
    {
        if (_isDestroyed || _isGamePaused) { return; }
        
        _timePassed += Time.deltaTime;
        if (_timePassed >= _lifetime) 
        {
            DeactivateBullet();
        } 
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.TryGetComponent<Enemy>(out var enemy) && !enemy.HadSwitchedToDestroyedMode)
        {
            enemy.TakeDamage(_damage);
            DeactivateBullet();
        }
    }
    
    private void DeactivateBullet() 
    {
        _isDestroyed = true;
        _timePassed = 0;
        _rb.velocity = Vector2.zero;
        OnDestroyed?.Invoke(this);
    }

    public void HandlePauseState(bool state)
    {
        _isGamePaused = state;
        if (state) 
        {
            _frozenVelocity = _rb.velocity;
            _frozenGravityScale = _rb.gravityScale;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
        }
        else
        {
            _rb.velocity = _frozenVelocity;
            _rb.gravityScale = _frozenGravityScale;
        }
    }

    public void PlayShootSound(AudioClip sound, float minPitch, float maxPitch) 
    {
        _audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        _audioSource.PlayOneShot(sound);
    }
}
