using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<bool> OnChangePauseState;
    public bool IsPaused { get; private set; }

    void Awake()
    {
        ServicesStorage.Instance.Register(this);
    }

    void Start() 
    {
        ChangePauseMode(false);
    }

    public void ChangePauseMode(bool isPaused) 
    {
        IsPaused = isPaused;
        OnChangePauseState?.Invoke(isPaused);
    }
}
