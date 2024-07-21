using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesManager : MonoBehaviour, IDataPersistence
{
    public event Action<UpgradeTypes, int> OnUpgraded;
    private Dictionary<UpgradeTypes, int> _upgrades;
    private Dictionary<UpgradeTypes, int> _maxUpgradeLevels;

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
    }

    public void LoadData(GameData data)
    {
        InitializeMaxUpgradeLevels();
        _upgrades = new Dictionary<UpgradeTypes, int>();
        
        foreach (var item in data.Upgrades)
        {
            _upgrades.TryAdd(item.Key, _maxUpgradeLevels != null && _maxUpgradeLevels.ContainsKey(item.Key) ? Mathf.Clamp(item.Value, 0, _maxUpgradeLevels[item.Key]) : item.Value);
            OnUpgraded?.Invoke(item.Key, _upgrades[item.Key]);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.Upgrades = _upgrades;
    }

    private void InitializeMaxUpgradeLevels() 
    {
        _maxUpgradeLevels = new Dictionary<UpgradeTypes, int>();
        var upgradableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IUpgradable>().ToList();
        foreach (var upgradableObject in upgradableObjects) 
        {
            bool isLoaded = _maxUpgradeLevels.TryAdd(upgradableObject.UpgradeType, upgradableObject.MaxUpgradeLevel);
            Debug.Log(isLoaded ? $"The object {upgradableObject.UpgradeType} was successfully initialized with max upgrade level of {upgradableObject.MaxUpgradeLevel}" 
                               : $"The object {upgradableObject.UpgradeType} wasn't successfully initialized");
        }
    }

    public int GetUpgradeLevel(UpgradeTypes type)
    {
        if (_upgrades.ContainsKey(type)) {
            return _upgrades[type];
        } 
        return default;
    }

    public void SetUpgradeLevel(UpgradeTypes type, int addPoints)
    {
        if (_upgrades.ContainsKey(type)) 
        {
            var currentUpgrade = _upgrades[type];

            if (currentUpgrade + addPoints <= _maxUpgradeLevels[type] &&
                currentUpgrade + addPoints >= 0)
            {
                _upgrades[type] += addPoints;
                OnUpgraded?.Invoke(type, _upgrades[type]);
            }
            else
            {
                return;
            }
        } 
    }
}

public enum UpgradeTypes 
{
    CannonLevel,
    TurretLevel,
    ArmorLevel,
    CoolingSystemLevel
}
