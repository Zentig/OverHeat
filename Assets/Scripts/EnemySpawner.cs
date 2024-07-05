using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _timeToSpawnEnemy;
    [SerializeField] private int _numberOfEnemyLines;
    [SerializeField] private float _offsetY = 4.75f;
    [SerializeField] private Vector3 _highestYSpawnPosition; // including this one*
    [SerializeField] private Transform _enemyStorage;
    [SerializeField] private List<Enemy> _enemyList;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _startEnemyCount = 5;
    private float _timePassed;
    private List<float> _possibleEnemySpawnPositions;
    private bool _isGamePaused;

    void Start()
    {
        GameManager.Instance.OnChangePauseState += HandlePauseState;

        _timePassed = _timeToSpawnEnemy - 0.5f;
        _possibleEnemySpawnPositions = new();
        for (int i = 0; i < _startEnemyCount; i++)
        {
            MakeNewEnemy();
        }
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
            SpawnEnemy(new Vector3(_highestYSpawnPosition.x, GetRandomSpawnPosY(), _highestYSpawnPosition.z));
            _timePassed = 0;
        } 
    }

    private Enemy MakeNewEnemy() 
    {
        Enemy newEnemy = Instantiate(_enemyPrefab, _enemyStorage);
        _enemyList.Add(newEnemy);
        newEnemy.gameObject.SetActive(false);
        newEnemy.OnDestroyed += ReturnEnemy;
        return newEnemy;
    }

    public void SpawnEnemy(Vector3 position) 
    {
        Enemy enemyToSpawn = _enemyList.Find(x => !x.gameObject.activeInHierarchy);
        if (enemyToSpawn == null) 
        {
            enemyToSpawn = MakeNewEnemy();
        }
        enemyToSpawn.gameObject.SetActive(true);
        enemyToSpawn.transform.position = new Vector3(position.x, position.y, position.z);
    }

    public void ReturnEnemy(Enemy enemy) 
    {
        enemy.gameObject.SetActive(false);
        ScoreManager.Instance.AddScore(enemy.WorthScore);
    }

    void OnDestroy() 
    {
        foreach (var item in _enemyList)
        {
            item.OnDestroyed -= ReturnEnemy;
        }
    }

    private float GetRandomSpawnPosY() 
    {
        if (_possibleEnemySpawnPositions.Count == 0) RefreshPossibleYPositions();

        int nextRoad = Random.Range(0, _possibleEnemySpawnPositions.Count - 1);
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