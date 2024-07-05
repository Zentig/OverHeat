using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _health;
    [field:SerializeField] public int MaxHealth { get; set; }
    public event Action<int> OnHPChanged;

    public int HP 
    { 
        get => _health; 
        set 
        { 
            _health = Mathf.Clamp(value, 0, MaxHealth);
            OnHPChanged?.Invoke(value);
        }
    }
}
