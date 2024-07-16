using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnPlayerKilled;
    [SerializeField] private Animator _explosionPrefab;
    private ShipTemperatureController _shipTemperatureController;
    private Health _healthReference;
    private GameManager _gameManager;
    private Animator _animator;

    void Start()
    {
        _shipTemperatureController = GetComponent<ShipTemperatureController>();
        _shipTemperatureController.OnOverheat += PlayDestroyAnimation;
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        OnPlayerKilled += _gameManager.GameOver;
        _animator = GetComponent<Animator>();
        _healthReference = GetComponent<Health>();
        _healthReference.OnHPChanged += HandleChangedHP;
    }

    private void OnDestroy()
    {
        _healthReference.OnHPChanged -= HandleChangedHP;
        _shipTemperatureController.OnOverheat -= PlayDestroyAnimation;
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
        Animator explosionAnim = Instantiate(_explosionPrefab, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Quaternion.identity);
        explosionAnim.SetTrigger("explosion");
        OnPlayerKilled?.Invoke();
        gameObject.SetActive(false);
    }
}
