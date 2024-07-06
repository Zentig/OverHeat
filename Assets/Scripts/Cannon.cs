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
    [SerializeField] private List<Bullet> _bulletList;
    private Enemy _nearestEnemy;
    private float _timePassed;
    private bool _pauseState;

    private void Start() 
    {
        _enemySpawner = ServicesStorage.Instance.Get<EnemySpawner>();
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _pauseState = false;
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
        if (_enemySpawner.ActiveEnemyStorage.Count == 0) { return; }

        (Enemy, float?) nearestEnemy = (default, null);
        Vector3 cannonPosition = transform.position;

        foreach (var enemy in _enemySpawner.ActiveEnemyStorage)
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

    private Bullet MakeNewBullet() 
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _bulletStorage);
        _bulletList.Add(newBullet);
        newBullet.Init();
        newBullet.gameObject.SetActive(false);
        newBullet.OnDestroyed += ReturnBullet;
        return newBullet;
    }

    public void SpawnBullet(Vector3 position, Quaternion rotation) 
    {
        Bullet bullet = _bulletList.Find(x => !x.gameObject.activeInHierarchy) ?? MakeNewBullet();
        bullet.gameObject.SetActive(true);
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.OnShoot();
    }

    public void ReturnBullet(Bullet bullet) 
    {
        bullet.gameObject.SetActive(false);
    }

    void OnDestroy() 
    {
        foreach (var item in _bulletList)
        {
            item.OnDestroyed -= ReturnBullet;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
    }
}
