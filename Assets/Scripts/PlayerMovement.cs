using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] public bool _isUp;
    [SerializeField] private float _rotationTargetScale;
    [SerializeField] private float _rotationTime;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;

    float r;
    private void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        SetDirection(_isUp);
    }

    public void InverseMovementDirection()
    {
        _isUp = !_isUp;
        SetDirection(_isUp);
    }

    public void FixedUpdate() 
    {
        float angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, _rotationTargetScale, ref r, _rotationTime);
        transform.rotation = Quaternion.Euler(new(0,0,angle));
    }

    private void SetDirection(bool direction) 
    {
        switch (direction)
        {
            case true:
                _currentDirection = Vector2.up;
                _rotationTargetScale = Math.Abs(_rotationTargetScale);
                break;
            case false:
                _currentDirection = Vector2.down;
                _rotationTargetScale = -Math.Abs(_rotationTargetScale);
                break;
        }
        
        _rb.velocity = _currentDirection * _speed;
    }
}

