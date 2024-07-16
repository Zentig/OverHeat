using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour, IDataPersistence
{
    // public event Action<UpgradeTypes, int> OnUpgraded;
    private Dictionary<UpgradeTypes, int> _upgrades;

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
    }

    public void LoadData(GameData data)
    {
        _upgrades = data.Upgrades;

        // foreach (var item in _upgrades.Keys)
        // {
        //     OnUpgraded?.Invoke(item, _upgrades[item]);
        // }
    }

    public void SaveData(ref GameData data)
    {
        data.Upgrades = _upgrades;
    }

    public int GetUpgradeLevel(UpgradeTypes type)
    {
        if (_upgrades.ContainsKey(type)) {
            return _upgrades[type];
        } 
        return default;
    }
}

public enum UpgradeTypes 
{
    CannonLevel,
    Buff2,
    Buff3,
    Defense
}