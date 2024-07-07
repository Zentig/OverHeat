using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;    
    [SerializeField] private GameManager _gameManager;  
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

    private void Start() 
    {
        _enemySpawner = ServicesStorage.Instance.Get<EnemySpawner>();
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _pauseState = false;

        _bulletPool = new GameObjectPool<Bullet>(_bulletPrefab, PreloadAction, null, (x) => {         
                x.OnDestroyed += _bulletPool.Return;
            },
            100, _bulletStorage);
        _bulletPool.StartPreload();
    }

    private Bullet PreloadAction()
    {
        var obj = Instantiate(_bulletPrefab, _bulletStorage);
        obj.OnDestroyed += _bulletPool.Return;
        obj.Init(); 
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
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
    }
}
