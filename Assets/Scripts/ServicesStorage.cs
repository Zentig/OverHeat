using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServicesStorage : MonoBehaviour
{
    public static ServicesStorage Instance { get; private set; }
    private readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();
    private readonly Dictionary<Type, List<object>> Containers = new Dictionary<Type, List<object>>();

    private void Awake() => Instance = this;

    public void Register<T>(T serviceInstance) 
    {
        if (Services.TryGetValue(typeof(T), out object service)) 
        { 
            if (service != null) Services.Remove(typeof(T));
        }
        Services.Add(typeof(T), serviceInstance);


        if (!Containers.TryGetValue(typeof(T), out List<object> containerList))
        {
            var list = new List<object> { serviceInstance };
            Containers.Add(typeof(T), list);
            return;
        }
        else 
        {
            containerList.Add(serviceInstance);
        }

        // if (Containers[typeof(T)] is not List<T> container) {
        //     var fullName = typeof(T).FullName;
        //     Debug.Log($"Containers[typeof({fullName})] is not List<{fullName}> container");
        // }
    }

    public T Get<T>() 
    {
        if (Services.ContainsKey(typeof(T))) return (T)Services[typeof(T)];
        return (T)default;
    }

    public List<T> GetAll<T>() 
    {
        if (Containers.ContainsKey(typeof(T)))
        {
            var objectList = Containers[typeof(T)];
            return objectList.Cast<T>().ToList();;
        }
        return null;
    }
}
