using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IUpgradable
{
    [field:SerializeField] public int UpgradeLevel { get; set; }
    [field:SerializeField] public int MaxUpgradeLevel { get; set; }
    [field:SerializeField] public UpgradeTypes UpgradeType { get; set; }
    [SerializeField] private GunConfig _config;
    [Header("Temperature")]
    [SerializeField] private float _temperatureAddValue;
    [Header("Sounds")]
    [SerializeField] private BulletSound _prefab;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;
    [Header("Shoot Configuration")]
    [SerializeField] private float _baseCooldown;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnBulletPos;
    [SerializeField] private float _distanceThreshold;
    [SerializeField] private float _minZRotation;
    [SerializeField] private float _maxZRotation;
    [SerializeField] private Transform _bulletStorage;
    [Header("Particles")]
    [SerializeField] private BulletParticle _particlePrefab;
    [SerializeField] private Transform _particleParent;
    [SerializeField] private AudioClip _particleSound;
    private Enemy _nearestEnemy;
    private float _timePassed;
    private bool _pauseState;
    private GameObjectPool<Bullet> _bulletPool;
    private GameObjectPool<BulletParticle> _particlePool;
    private GameObjectPool<BulletSound> _soundPool;
    private Transform _soundParent;
    [Header("Managers")]
    private List<EnemySpawner> _enemySpawnerList;    
    private GameManager _gameManager;  
    private AudioManager _audioManager;
    private UpgradesManager _upgradesManager;
    private ShipTemperatureController _shipTemperatureController;
    private Func<float, int, float> _calculateCooldownFunc;
    private float _cooldown;

    private void OnEnable() 
    {
        _upgradesManager = ServicesStorage.Instance.Get<UpgradesManager>();
        _upgradesManager.OnUpgraded += HandleUpgraded;
    }

    private void Start() 
    {
        _calculateCooldownFunc ??= _config.CalculateCooldown;
        _cooldown = _calculateCooldownFunc(_baseCooldown, UpgradeLevel);

        _enemySpawnerList = ServicesStorage.Instance.GetAll<EnemySpawner>();
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
        _shipTemperatureController = ServicesStorage.Instance.Get<ShipTemperatureController>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _pauseState = false;

        _soundParent = new GameObject("BulletSoundPool").transform;
        _soundPool = new GameObjectPool<BulletSound>(
            () => {
                var x = Instantiate(_prefab, _soundParent); 
                x.Init();
                x.OnDestroyed += _soundPool.Return;
                _gameManager.OnChangePauseState += x.HandlePauseState;
                return x;
            },
            (x) => { x.PlayShootSound(_audioManager.GetCurrentSFXVolume(), _shootSound, _minPitch, _maxPitch); },
            null, 100
        );
        _soundPool.StartPreload();

        _bulletPool = new GameObjectPool<Bullet>(BulletPreloadAction, (x) => {
            _soundPool.Get();
        }, (x) => {         
                _particlePool.Get(x.transform.position);
            },
            100);
        _bulletPool.StartPreload();

        _particlePool = new GameObjectPool<BulletParticle>(ParticlePreloadAction, null, null, 100);    
        _particlePool.StartPreload();
    }

    private void HandleUpgraded(UpgradeTypes type, int level)
    { 
        if (type == UpgradeType) 
        {
            if (level <= 0) 
            {
                gameObject.SetActive(false);
                Unsubscribe();
                return;
            }
            UpgradeLevel = level;
            _calculateCooldownFunc ??= _config.CalculateCooldown;
            _cooldown = _calculateCooldownFunc(_baseCooldown, UpgradeLevel);
        }
    }

    private Bullet BulletPreloadAction()
    {
        var bullet = Instantiate(_bulletPrefab, _bulletStorage);
        bullet.OnDestroyed += _bulletPool.Return;
        _gameManager.OnChangePauseState += bullet.HandlePauseState;
        bullet.Init(_config); 
        return bullet;
    }

    private BulletParticle ParticlePreloadAction()
    {
        var obj = Instantiate(_particlePrefab, _particleParent);
        obj.OnDestroyed += _particlePool.Return;
        return obj;
    }

    private void HandlePauseState(bool pauseState) => _pauseState = pauseState;

    private void Update() 
    {
        if (_pauseState) return;

        Rotation();

        _timePassed += Time.deltaTime;
        if (_timePassed >= _cooldown) 
        {
            if (_nearestEnemy != null && _nearestEnemy.isActiveAndEnabled) Shoot();
            _timePassed = 0;
        } 
    }

    private void Rotation() 
    {
        (Enemy, float?) nearestEnemy = (default, null);
        Vector3 cannonPosition = transform.position;
        float angleDifference = 180;

        foreach (var enemySpawner in _enemySpawnerList)
        {
            if (enemySpawner.Pool.ActiveObjects.Count == 0) { continue; }

            foreach (var enemy in enemySpawner.Pool.ActiveObjects)
            {
                if (enemy == null) continue;

                Vector3 enemyPosition = enemy.transform.position;
                float distanceToNextEnemy = Vector2.Distance(enemyPosition, cannonPosition);
                angleDifference = GetAngleBetweenTwoPositions(transform.position + (Vector3)Vector2.right, enemy.transform.position);

                if ((nearestEnemy.Item2 == null || angleDifference < nearestEnemy.Item2) && distanceToNextEnemy < _distanceThreshold 
                         && angleDifference > _minZRotation && angleDifference < _maxZRotation)  
                {
                    nearestEnemy = (enemy,angleDifference);
                }
            }
        }
        _nearestEnemy = nearestEnemy.Item1;
        transform.rotation = Quaternion.Euler(0, 0, angleDifference);
    }

    private float GetAngleBetweenTwoPositions(Vector3 first, Vector3 second) => Mathf.Atan2(second.y - first.y, second.x - first.x) * Mathf.Rad2Deg;

    private void Shoot() 
    {
        SpawnBullet(_spawnBulletPos.position, transform.rotation);
        _shipTemperatureController.AddTemperature(_temperatureAddValue*(UpgradeLevel+1));
    }

    public void SpawnBullet(Vector3 position, Quaternion rotation) 
    {
        var b = _bulletPool.Get(position, rotation);
        b.OnShoot(UpgradeLevel);
    }

    private void OnDestroy() => Unsubscribe();

    private void OnDisable() => Unsubscribe();

    void Unsubscribe() 
    {
        _upgradesManager.OnUpgraded -= HandleUpgraded;
        if (_bulletPool != null) {
            foreach (var bullet in _bulletPool.PoolQueue)
            {
                bullet.OnDestroyed -= _bulletPool.Return;
                _gameManager.OnChangePauseState -= bullet.HandlePauseState;
            }
        }
        if (_particlePool != null) {
            foreach (var particle in _particlePool.PoolQueue)
            {
                particle.OnDestroyed -= _particlePool.Return;
            }
        }
        if (_soundPool != null) {
            foreach (var sound in _soundPool.PoolQueue)
            {
                sound.OnDestroyed -= _soundPool.Return;
                _gameManager.OnChangePauseState -= sound.HandlePauseState;
            }
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.right);
        Gizmos.color = Color.blue;
        if (_nearestEnemy != null && _nearestEnemy.isActiveAndEnabled) Gizmos.DrawLine(_nearestEnemy.transform.position, transform.position);
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, new Vector2(Mathf.Cos(_maxZRotation * Mathf.Deg2Rad), Mathf.Sin(_maxZRotation * Mathf.Deg2Rad)*2));
        // Gizmos.DrawLine(transform.position, new Vector2(Mathf.Cos(_minZRotation * Mathf.Deg2Rad), Mathf.Sin(_minZRotation * Mathf.Deg2Rad)*2));
    }
}
