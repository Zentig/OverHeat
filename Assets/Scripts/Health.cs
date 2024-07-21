using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _health;
    [field:SerializeField] public float MaxHealth { get; set; }
    public event Action<float> OnHPChanged;

    public float HP 
    { 
        get => _health; 
        set 
        { 
            if (value > MaxHealth) _health = MaxHealth;
            else _health = value;

            OnHPChanged?.Invoke(value);
        }
    }
}
