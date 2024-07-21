using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Armor : MonoBehaviour
{
    [SerializeField] private int _armor;
    
    [field: SerializeField] public int MaxArmor { get; set; }
    public event Action<int> OnArmorChanged;

    public int CurrentArmor
    {
        
        get => _armor;
        set
        {
            if (value > MaxArmor) _armor = MaxArmor;
            else _armor = value;
            _armor = Mathf.Clamp(value, 0, MaxArmor);
            OnArmorChanged?.Invoke(value);
        }
    }


}
