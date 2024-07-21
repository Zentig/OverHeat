using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    public GameObjectPool<T> Pool { get; private set; }
    [SerializeField] protected T _prefab;
    [SerializeField] protected float _timeToSpawn;
    [SerializeField] protected int _preloadCount = 20;
    protected Transform _storage;
    protected float _timePassed;
    protected bool _isGamePaused;

    protected virtual void Start()
    {
        _storage = new GameObject($"{typeof(T).FullName}Pool").transform;
        _timePassed = _timeToSpawn - 0.1f;
        Pool = new GameObjectPool<T>(PreloadAction, GetAction, ReturnAction, _preloadCount);
        Pool.StartPreload();
    }

    protected abstract T PreloadAction();
    protected abstract void GetAction(T obj);
    protected abstract void ReturnAction(T obj);
    protected abstract void Spawn();

    protected virtual void Update()
    {
        if (_isGamePaused) { return; }
        
        _timePassed += Time.deltaTime;
        if (_timePassed >= _timeToSpawn) 
        {
            Spawn();
            _timePassed = 0;
        } 
    }

}