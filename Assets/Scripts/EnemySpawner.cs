using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObjectPool<Enemy> EnemyPool { get; private set; }
    [SerializeField] private float _timeToSpawnEnemy;
    [SerializeField] private int _numberOfEnemyLines;
    [SerializeField] private float _offsetY = 4.75f;
    [SerializeField] private Vector3 _highestYSpawnPosition; // including this one*
    [SerializeField] private Transform _enemyStorage;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Enemy _enemyPrefab2;
    [SerializeField] private int _startEnemyCount = 20;

    private float _timePassed;
    private List<float> _possibleEnemySpawnPositions;
    private bool _isGamePaused;
    private GameManager _gameManager;
    private ScoreManager _scoreManager;

    private void Awake() => ServicesStorage.Instance.Register(this);

    void Start()
    {
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnChangePauseState += HandlePauseState;
        _scoreManager = ServicesStorage.Instance.Get<ScoreManager>();

        _timePassed = _timeToSpawnEnemy - 0.5f;
        _possibleEnemySpawnPositions = new();

        EnemyPool = new GameObjectPool<Enemy>(PreloadAction,
            null, (x) => { 
                _scoreManager.AddScore(x.WorthScore); 
            },
            _startEnemyCount);
        EnemyPool.StartPreload();
    }

    private Enemy PreloadAction()
    {
        var obj = UnityEngine.Random.Range(0,2) == 0 ? Instantiate(_enemyPrefab, _enemyStorage) : Instantiate(_enemyPrefab2, _enemyStorage);
        obj.OnDestroyed += EnemyPool.Return;
        return obj;
    }

    private void HandlePauseState(bool pauseState)
    {
        _isGamePaused = pauseState;
        _enemyStorage.gameObject.SetActive(!pauseState);
    }

    void Update()
    {
        if (_isGamePaused) { return; }
        
        _timePassed += Time.deltaTime;
        if (_timePassed >= _timeToSpawnEnemy) 
        {
            EnemyPool.Get(new Vector3(_highestYSpawnPosition.x, GetRandomSpawnPosY(), _highestYSpawnPosition.z));
            _timePassed = 0;
            _timeToSpawnEnemy -= _timeToSpawnEnemy/350;
        } 
    }

    void OnDestroy() 
    {
        foreach (var item in EnemyPool.PoolQueue)
        {
            item.OnDestroyed -= EnemyPool.Return;
        }
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
            _possibleEnemySpawnPositions.Add(_highestYSpawnPosition.y - (i * _offsetY));
        }
    }
}