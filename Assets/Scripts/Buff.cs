using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Buff : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _direction = Vector2.left;
    protected Rigidbody2D _rb;
    protected GameManager _gameManager;
    protected bool _isGamePaused;
    
    private void Start() 
    {
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
    }

    public virtual void Init() => _rb = GetComponent<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("enemyFear")) 
        {
            gameObject.SetActive(false);
            HandleDestroyed();
        }
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            OnCollisionWithPlayer(player);
            gameObject.SetActive(false);
            HandleDestroyed();
        }
    }

    public abstract void OnCollisionWithPlayer(Player player);
    protected abstract void HandleDestroyed();

    private void HandlePauseState(bool pauseState)
    {
        _isGamePaused = pauseState;
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        _rb.velocity =  !_isGamePaused ? _direction * _speed : new(0, 0);
    }
}
