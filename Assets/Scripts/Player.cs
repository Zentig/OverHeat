using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnPlayerKilled;
    public event Action ShakingCam; // Мій Евент для камери.
    public event Action OnPlayerLostAllArmor;
    private event Action<float> OnPlayerDamaged; 
    [SerializeField] private float _takenDamage;
    [SerializeField] private float _damagePerHit;
    [SerializeField] private Animator _explosionPrefab;
    private ShipTemperatureController _shipTemperatureController;
    private Armor _armorReference;
    private GameManager _gameManager;
    private Animator _animator;

    private void OnEnable()
    {
        ServicesStorage.Instance.Register(this);
    }

    void Start()
    {
        _shipTemperatureController = GetComponent<ShipTemperatureController>();
        _shipTemperatureController.OnOverheat += PlayDestroyAnimation;
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        OnPlayerKilled += _gameManager.GameOver;
        _animator = GetComponent<Animator>();
        _armorReference = GetComponent<Armor>();
        _armorReference.OnArmorChanged += HandleChangedArmor;
    }

    private void OnDestroy()
    {
        _armorReference.OnArmorChanged -= HandleChangedArmor;
        _shipTemperatureController.OnOverheat -= PlayDestroyAnimation;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy) && !enemy.HadSwitchedToDestroyedMode) 
        {
            _armorReference.CurrentArmor -= enemy.Damage;
            
            enemy.SwitchToDestroyAnimationMode();

            ShakingCam?.Invoke(); // Викликаю потрясіння для камери.
        }
        if (other.gameObject.tag == "Death") 
        {
            PlayExplosionAnimation();
        }
    }
    private void HandleChangedArmor(int newArmor)
    {
        if (newArmor < 0)
        {
            _takenDamage+=_damagePerHit; 
        }
    }
    public float TotalDamageTaken
    {
        get => _takenDamage;
        set
        {
            OnPlayerDamaged?.Invoke(value);
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
