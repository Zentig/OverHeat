using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field:SerializeField] public int WorthScore { get; private set; }
    [field:SerializeField] public int Damage { get; set; }
    [SerializeField] private float _speed;
    public event Action<Enemy> OnDestroyed;
    private Rigidbody2D _rb;
    public bool HadCollisionWithPlayer { get; private set; } = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rb.velocity = !HadCollisionWithPlayer ? Vector2.left * _speed : new(0,0);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "enemyFear") 
        {
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this);
            ResetCollision();
        }
    }
    void ResetCollision() {
        _rb.gravityScale = 0;
        HadCollisionWithPlayer = false;
    }
    public void SwitchToDestroyedMode() 
    {
        _rb.gravityScale = 35;
        HadCollisionWithPlayer = true;
    }
}
