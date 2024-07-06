using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field:SerializeField] public int WorthScore { get; private set; }
    [field:SerializeField] public int Damage { get; set; }
    [SerializeField] private float _speed;
    private Health _health;
    public event Action<Enemy> OnDestroyed;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    public bool HadSwitchedToDestroyedMode { get; private set; } = false;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _health = GetComponent<Health>();
        _health.OnHPChanged += HandleChangedHP;
    }

    void FixedUpdate()
    {
        _rb.velocity = !HadSwitchedToDestroyedMode ? Vector2.left * _speed : new(0,0);
    }

    private void HandlePauseState(bool pauseState) 
    {
        if (!pauseState) 
        {
            Disable();
        }
    }

    private void HandleChangedHP(int hp) 
    {
        if (hp < 0) 
        {
            SwitchToDestroyAnimationMode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("enemyFear")) 
        {
            Disable();
            ResetCollision();
        }
    }
    private void ResetCollision() 
    {
        _rb.gravityScale = 0;
        HadSwitchedToDestroyedMode = false;
    }

    public void SwitchToDestroyAnimationMode() 
    {
        _rb.gravityScale = 35;
        HadSwitchedToDestroyedMode = true;
        _collider.isTrigger = true;
    }

    private void Disable() 
    {
        gameObject.SetActive(false);
        _health.HP = _health.MaxHealth;
        _collider.isTrigger = false;
        transform.rotation = Quaternion.identity;
        OnDestroyed?.Invoke(this);
    }
    public void TakeDamage(int damage)
    {
        _health.HP -= damage;
    }
}
