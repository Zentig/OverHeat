using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectPool<T> where T : MonoBehaviour
{
    public Queue<T> PoolQueue { get; protected set; }
    public List<T> ActiveObjects { get; protected set; }
    private readonly Action<T> _returnAction;
    private readonly int _preloadCount;
    private readonly Action<T> _getAction;
    private readonly Func<T> _preloadAction;

    public GameObjectPool(Func<T> preloadAction, Action<T> getAction, Action<T> returnAction, int preloadCount)
    {
        PoolQueue = new Queue<T>();
        ActiveObjects = new List<T>();

        _preloadAction = preloadAction;
        _getAction = getAction;
        _returnAction = returnAction;
        _preloadCount = preloadCount;
    }

    public void StartPreload() 
    {
        for (int i = 0; i < _preloadCount; i++) 
        {
            Preload(true);
        }
    }

    public T Preload(bool isPooled)
    {
        var obj = _preloadAction();
        if (isPooled) PoolQueue.Enqueue(obj);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public T Get() 
    {
        T obj = PoolQueue.Count > 0 ? obj = PoolQueue.Dequeue() : obj = Preload(false);
        obj.gameObject.SetActive(true);
        _getAction?.Invoke(obj);
        ActiveObjects.Add(obj);
        return obj;
    }

    public T Get(Vector3 position)
    {
        var obj = Get();
        obj.transform.position = position;
        return obj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        var obj = Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void Return(T obj)
    {
        _returnAction?.Invoke(obj);
        PoolQueue.Enqueue(obj);
        ActiveObjects.Remove(obj);
        obj.gameObject.SetActive(false);
    }

    public void ReturnAll()
    {
        for (int i = 0; i < ActiveObjects.Count; i++)
        {
            Return(ActiveObjects[i]);
        }
    }

}
