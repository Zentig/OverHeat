using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;    
    [SerializeField] private GameManager _gameManager;  
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnBulletPos;
    [SerializeField] private float _shootTime;
    [SerializeField] private float _distanceThreshold;
    [SerializeField] private float _minZRotation;
    [SerializeField] private float _maxZRotation;
    [SerializeField] private Transform _bulletStorage;
    private Enemy _nearestEnemy;
    private float _timePassed;
    private bool _pauseState;
    private GameObjectPool<Bullet> _bulletPool;
    private GameObjectPool<BulletParticle> _particlePool;
    [Header("Particles")]
    [SerializeField] private BulletParticle _particlePrefab;
    [SerializeField] private Transform _particleParent;
    [SerializeField] private AudioClip _particleSound;

    private void Start() 
    {
        _enemySpawner = ServicesStorage.Instance.Get<EnemySpawner>();
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _pauseState = false;

        _bulletPool = new GameObjectPool<Bullet>(BulletPreloadAction, (x) => _audioManager.PlayOneShot(_shootSound, 0.5f, 0.9f, 0.55f), (x) => {         
                x.OnDestroyed += _bulletPool.Return;
                _particlePool.Get(x.transform.position);
            },
            100);
        _bulletPool.StartPreload();

        _particlePool = new GameObjectPool<BulletParticle>(ParticlePreloadAction, null, null/*(x) => _audioManager.PlayOneShot(_particleSound, 0.7f, 1.1f)*/, 100);    
        _particlePool.StartPreload();
    }

    private Bullet BulletPreloadAction()
    {
        var obj = Instantiate(_bulletPrefab, _bulletStorage);
        obj.OnDestroyed += _bulletPool.Return;
        obj.Init(); 
        return obj;
    }

    private BulletParticle ParticlePreloadAction()
    {
        var obj = Instantiate(_particlePrefab, _particleParent);
        obj.OnDestroyed += _particlePool.Return;
        return obj;
    }

    private void HandlePauseState(bool pauseState) 
    {
        _bulletStorage.gameObject.SetActive(!pauseState);
        _pauseState = pauseState;
    }

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

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,Mathf.Clamp(Mathf.Atan2(_nearestEnemy.transform.position.y - transform.position.y, 
                                            _nearestEnemy.transform.position.x - transform.position.x)*Mathf.Rad2Deg,_minZRotation,_maxZRotation)), 0.5f);
    }

    private void Shoot() 
    {
        SpawnBullet(_spawnBulletPos.position, transform.rotation);
    }

    public void SpawnBullet(Vector3 position, Quaternion rotation) 
    {
        var b = _bulletPool.Get(position, rotation);
        b.OnShoot();
    }

    void OnDestroy() 
    {
        foreach (var item in _bulletPool.PoolQueue)
        {
            item.OnDestroyed -= _bulletPool.Return;
        }
        foreach (var item in _particlePool.PoolQueue)
        {
            item.OnDestroyed -= _particlePool.Return;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
    }
}
