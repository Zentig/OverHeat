using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private Enemy _prefab2;
    [SerializeField] private int _numberOfEnemyLines;
    [SerializeField] private float _offsetY = 4.75f;
    [SerializeField] private Vector3 _highestYSpawnPosition; 
    private List<float> _possibleEnemySpawnPositions;
    private GameManager _gameManager;
    private ScoreManager _scoreManager;
    
    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _scoreManager = ServicesStorage.Instance.Get<ScoreManager>();
    }

    protected override void Start()
    {
        base.Start();
        _possibleEnemySpawnPositions = new();
    }

    private void OnDestroy() 
    {
        _gameManager.OnChangePauseState -= HandlePauseState;

        foreach (var item in Pool.PoolQueue)
        {
            item.OnDestroyed -= Pool.Return;
        }
    }

    protected override Enemy PreloadAction()
    {
        var obj = UnityEngine.Random.Range(0,2) == 0 ? Instantiate(_prefab, _storage) : Instantiate(_prefab2, _storage);
        obj.OnDestroyed += Pool.Return;
        return obj;
    }

    protected override void GetAction(Enemy obj)
    {
        obj.UpdateSpeed();
        obj.SetDefaultAnimatorSpeed();
    }

    protected override void ReturnAction(Enemy obj) => _scoreManager.AddScore(obj.WorthScore);

    protected override void Spawn() => Pool.Get(new Vector3(_highestYSpawnPosition.x, GetRandomSpawnPosY(), _highestYSpawnPosition.z));

    private void HandlePauseState(bool pauseState) => _isGamePaused = pauseState;

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
            _possibleEnemySpawnPositions.Add(_highestYSpawnPosition.y - (i * _offsetY));
        }
    }
}