using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServicesStorage : MonoBehaviour
{
    public static ServicesStorage Instance { get; private set; }
    private readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

    private void Awake() 
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Register<T>(T serviceInstance) 
    {
        if (Services.TryGetValue(typeof(T), out object service)) 
        { 
            if (service != null) Services.Remove(typeof(T));
        }
        Services.Add(typeof(T), serviceInstance);
    }

    public T Get<T>() 
    {
        if (Services.ContainsKey(typeof(T))) return (T)Services[typeof(T)];
        return (T)default;
    }
}
