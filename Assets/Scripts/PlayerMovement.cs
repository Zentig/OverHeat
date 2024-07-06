using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Animator _gameOverAnimator;
    private Health _healthReference;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;
    private bool _isPaused;
    private GameManager _gameManager;

    float r;

    [SerializeField] private ShipTemperatureController _shipTemperatureController;

    private void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        SetDirection(IsGoingUp);
        _healthReference = GetComponent<Health>();
        _healthReference.OnHPChanged += HandleChangedHP;
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
    }

    private void HandlePauseState(bool pauseState) 
    {
        _isPaused = pauseState;
        _rb.velocity = pauseState ? new(0,0) : _currentDirection * _speed;
    }

    private void OnDestroy() 
    {
        _healthReference.OnHPChanged -= HandleChangedHP;
        _gameManager.OnChangePauseState -= HandlePauseState;
    }

    public void InverseMovementDirection()
    {
        IsGoingUp = !IsGoingUp;
        SetDirection(IsGoingUp);
    }

    public void FixedUpdate() 
    {
        if (_isPaused) return; 
        float angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, _rotationTargetScale, ref r, _rotationTime);
        transform.rotation = Quaternion.Euler(new(0,0,angle));

        _speed = _maxSpeed / (1 + (_shipTemperatureController.currentTemperature / _shipTemperatureController.maxTemperature));
        _rotationTime = _minRotationTime * (1.1f + (_shipTemperatureController.currentTemperature / _shipTemperatureController.maxTemperature));

        if (_shipTemperatureController.currentTemperature >= _shipTemperatureController.maxTemperature)
        {
            Debug.Log("explosion");
        }
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

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy) && !enemy.HadSwitchedToDestroyedMode) 
        {
            _healthReference.HP -= enemy.Damage;
            enemy.SwitchToDestroyAnimationMode();
        }
    }

    private void HandleChangedHP(int newHP) 
    {
        if (newHP <= 0) 
        {
            Debug.Log("Game over!");
            _gameManager.ChangePauseMode(true);
            _gameOverAnimator.SetBool("gameOver", true);
        }
    }
}

