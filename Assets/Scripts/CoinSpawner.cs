using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : Spawner<Coin>
{    
    [SerializeField] private int _numberOfEnemyLines = 4;
    [SerializeField] private float _offsetY = 4.75f;
    [SerializeField] private Vector3 _highestSpawnPosition; 
    private List<float> _possibleEnemySpawnPositions;

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
        _possibleEnemySpawnPositions = new();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (Coin item in Pool.PoolQueue)
        {
            item.OnDestroyed -= Pool.Return;
        }
    }
    
    protected override Coin PreloadAction()
    {
        var obj = Instantiate(_prefab, _storage);
        obj.Init();
        obj.OnDestroyed += Pool.Return;
        return obj;
    }

    protected override void GetAction(Coin obj)
    {
        obj.UpdateSpeed();
    }

    protected override void ReturnAction(Coin obj) {  }

    protected override void Spawn()
    {
        Pool.Get(new Vector3(_highestSpawnPosition.x, GetRandomSpawnPosY(), _highestSpawnPosition.z));
    }

    private float GetRandomSpawnPosY() 
    {
        if (_possibleEnemySpawnPositions.Count == 0) RefreshPossibleYPositions();

        int nextRoad = UnityEngine.Random.Range(0, _possibleEnemySpawnPositions.Count - 1);
        var nextEnemy = _possibleEnemySpawnPositions[nextRoad];
        _possibleEnemySpawnPositions.Remove(nextEnemy);
        return nextEnemy;
    }

    private void RefreshPossibleYPositions() 
    {
        for (int i = 0; i < _numberOfEnemyLines; i++)
        {
            _possibleEnemySpawnPositions.Add(_highestSpawnPosition.y - (i * _offsetY));
        }
    }
}
