using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnDestroyed;
    [field:SerializeField] public int WorthScore { get; private set; }
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rb.velocity = Vector2.left * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "enemyFear") 
        {
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this);
        }
    }
}
