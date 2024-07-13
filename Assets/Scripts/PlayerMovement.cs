using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public event Action OnPlayerKilled;
    [field:SerializeField] public bool IsGoingUp { get; private set; }
    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _rotationTargetScale;
    [SerializeField] private float _rotationTime;
    [SerializeField] private float _minRotationTime;
    [SerializeField] private Animator _explosionAnimator;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private ShipTemperatureController _shipTemperatureController;
    private Animator _animator;
    private Health _healthReference;
    private Rigidbody2D _rb;
    private Vector2 _currentDirection;
    private bool _isPaused;
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private float _sfxVolume;

    float r;
    private async void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        SetDirection(IsGoingUp);
        _healthReference = GetComponent<Health>();
        _healthReference.OnHPChanged += HandleChangedHP;
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        OnPlayerKilled += _gameManager.GameOver;
        await Task.Delay(1);
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
        _audioManager.OnSFXVolumeChanged += (value) => _audioSource.volume = value;
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

        if (_shipTemperatureController.CurrentTemperature >= _shipTemperatureController.MaxTemperature)
        {
            if (!_animator.GetBool("gameOver")) PlayDestroyAnimation();
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
        if (other.gameObject.tag == "Death") 
        {
            PlayExplosionAnimation();
        }
    }

    private void HandleChangedHP(int newHP) 
    {
        if (newHP <= 0) 
        {
            Debug.Log("Game over!");
            OnPlayerKilled?.Invoke();
        }
    }

    public void PlayDestroyAnimation() 
    {
        _animator.SetBool("gameOver", true);
    }

    public void PlayExplosionAnimation() 
    {
        _audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.3f);
        _audioSource.PlayOneShot(_explosionSound);
        Animator explosionAnim = Instantiate(_explosionAnimator, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Quaternion.identity);
        explosionAnim.SetTrigger("explosion");
        OnPlayerKilled?.Invoke();
        gameObject.SetActive(false);
    }
}

