using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _lifetime = 4f;
    public event Action<Bullet> OnDestroyed;
    private Rigidbody2D _rb;
    private float _timePassed = 0;
    private bool _isDestroyed = false;

    public void Init() 
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnShoot() 
    {
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
        if (_isDestroyed) { return; }
        
        _timePassed += Time.deltaTime;
        if (_timePassed >= _lifetime) 
        {
            DeactivateBullet();
        } 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            Debug.Log(enemy.name + " was shot!");
            DeactivateBullet();
        }
    }
    
    private void DeactivateBullet() 
    {
        _isDestroyed = true;
        _timePassed = 0;
        OnDestroyed?.Invoke(this);
    }
}
