using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IUpgradable
{
    [field:SerializeField] public int UpgradeLevel { get; set; }
    [field:SerializeField] public int MaxUpgradeLevel { get; set; }
    [field:SerializeField] public UpgradeTypes UpgradeType { get; set; }
    [Header("Temperature")]
    [SerializeField] private float _temperatureAddValue;
    [Header("Sounds")]
    [SerializeField] private BulletSound _prefab;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;
    [Header("Shoot Configuration")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnBulletPos;
    [SerializeField] private float _shootTime;
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
    private EnemySpawner _enemySpawner;    
    private GameManager _gameManager;  
    private AudioManager _audioManager;
    private UpgradesManager _upgradesManager;
    private ShipTemperatureController _shipTemperatureController;

    private int GetCurrentUpgradeLevel() => UpgradeLevel;

    private void OnEnable() 
    {
        _upgradesManager = ServicesStorage.Instance.Get<UpgradesManager>();
        _upgradesManager.OnUpgraded += HandleUpgraded;
    }

    private void Start() 
    {
        _enemySpawner = ServicesStorage.Instance.Get<EnemySpawner>();
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
        if (type == UpgradeType) UpgradeLevel = level;
    }

    private Bullet BulletPreloadAction()
    {
        var bullet = Instantiate(_bulletPrefab, _bulletStorage);
        bullet.OnDestroyed += _bulletPool.Return;
        _gameManager.OnChangePauseState += bullet.HandlePauseState;
        bullet.Init(); 
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
        if (_timePassed >= _shootTime) 
        {
            Shoot();
            _timePassed = 0;
        } 
    }

    private void Rotation() 
    {
        if (_enemySpawner.EnemyPool.ActiveObjects.Count == 0) { return; }

        (Enemy, float?) nearestEnemy = (default, null);
        Vector3 cannonPosition = transform.position;

        foreach (var enemy in _enemySpawner.EnemyPool.ActiveObjects)
        {
            Vector3 enemyPosition = enemy.transform.position;
            float distanceToNextEnemy = Mathf.Sqrt((enemyPosition.x - cannonPosition.x)*(enemyPosition.x - cannonPosition.x) + 
                                                   (enemyPosition.y - cannonPosition.y)*(enemyPosition.y - cannonPosition.y));
            if ((nearestEnemy.Item2 == null || distanceToNextEnemy < nearestEnemy.Item2) && distanceToNextEnemy < _distanceThreshold)  
            {
                nearestEnemy = (enemy,distanceToNextEnemy);
            }
        }
        _nearestEnemy = nearestEnemy.Item1;
        
        if (nearestEnemy.Item2 == null) return;

        // transform.rotation = Quaternion.Euler(0,0,Mathf.Clamp(Mathf.Atan2(_nearestEnemy.transform.position.y - transform.position.y, 
        //                                     _nearestEnemy.transform.position.x - transform.position.x)*Mathf.Rad2Deg,_minZRotation,_maxZRotation));
        transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(_nearestEnemy.transform.position.y - transform.position.y, 
                                            _nearestEnemy.transform.position.x - transform.position.x)*Mathf.Rad2Deg);
    }

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

    void OnDestroy() 
    {
        foreach (var bullet in _bulletPool.PoolQueue)
        {
            bullet.OnDestroyed -= _bulletPool.Return;
            _gameManager.OnChangePauseState -= bullet.HandlePauseState;
        }
        foreach (var particle in _particlePool.PoolQueue)
        {
            particle.OnDestroyed -= _particlePool.Return;
        }
        foreach (var sound in _soundPool.PoolQueue)
        {
            sound.OnDestroyed -= _soundPool.Return;
            _gameManager.OnChangePauseState -= sound.HandlePauseState;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
    }
}
