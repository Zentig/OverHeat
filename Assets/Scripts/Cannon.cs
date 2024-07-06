using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;    
    private Enemy _nearestEnemy;

    private void Start() 
    {
        _enemySpawner = ServicesStorage.Instance.Get<EnemySpawner>();
    }

    private void Update() 
    {
        if (_enemySpawner.ActiveEnemyStorage.Count == 0) { return; }

        (Enemy, float) nearestEnemy = (default, Mathf.Infinity);
        Vector3 cannonPosition = transform.position;

        foreach (var enemy in _enemySpawner.ActiveEnemyStorage)
        {
            Vector3 enemyPosition = enemy.transform.position;
            float distanceToNextEnemy = Mathf.Sqrt((enemyPosition.x - cannonPosition.x)*(enemyPosition.x - cannonPosition.x) + 
                                                   (enemyPosition.y - cannonPosition.y)*(enemyPosition.y - cannonPosition.y));
            if (distanceToNextEnemy < nearestEnemy.Item2) 
            {
                nearestEnemy = (enemy,distanceToNextEnemy);
            }
        }
        _nearestEnemy = nearestEnemy.Item1;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,Mathf.Atan2(_nearestEnemy.transform.position.y - transform.position.y, 
                                                                                _nearestEnemy.transform.position.x - transform.position.x)*Mathf.Rad2Deg), 0.5f);
    }
}
