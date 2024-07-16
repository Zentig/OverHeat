using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [field:SerializeField] public bool IsGoingUp { get; private set; }
    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _rotationTargetScale;
    [SerializeField] private float _rotationTime;
    [SerializeField] private float _minRotationTime;
    private ShipTemperatureController _shipTemperatureController;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;
    private bool _isPaused;
    private GameManager _gameManager;

    float r;
    private void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _shipTemperatureController = GetComponent<ShipTemperatureController>();
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        SetDirection(IsGoingUp);
    }

    private void HandlePauseState(bool pauseState) 
    {
        _isPaused = pauseState;
        _rb.velocity = pauseState ? new(0,0) : _currentDirection * _speed;
    }

    private void OnDestroy() => _gameManager.OnChangePauseState -= HandlePauseState;

    public void InverseMovementDirection()
    {
        IsGoingUp = !IsGoingUp;
        SetDirection(IsGoingUp);
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space)) InverseMovementDirection();
    }

    public void FixedUpdate() 
    {
        if (_isPaused) return; 
        float angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, _rotationTargetScale, ref r, _rotationTime);
        transform.rotation = Quaternion.Euler(new(0,0,angle));

        _speed = _maxSpeed / (1 + (_shipTemperatureController.CurrentTemperature / _shipTemperatureController.MaxTemperature));
        _rotationTime = _minRotationTime * (1.1f + (_shipTemperatureController.CurrentTemperature / _shipTemperatureController.MaxTemperature));    
    }

    private void SetDirection(bool direction) 
    {
        if (_isPaused) return;
      
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

