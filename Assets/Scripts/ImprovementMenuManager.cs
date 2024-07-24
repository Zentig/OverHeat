using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImprovementMenuManager : MonoBehaviour
{
    private UpgradesManager _upgradesManager;

    private void OnEnable() => _upgradesManager = ServicesStorage.Instance.Get<UpgradesManager>();

    public void LoadMenu() => Debug.Log("Loading main menu.");

    public void LeaveWorkshop() => SceneManager.LoadScene(2);

    public void MakeUpgradeByType(string upgradeType) 
    {
        if (_upgradesManager == null) 
        {
            Debug.Log($"UpgradesManager is null. Can't upgrade {upgradeType}. Return!");
            return;
        }

        if (Enum.TryParse(upgradeType, out UpgradeTypes upgradeTypeEnum)) 
        {
            _upgradesManager.AddUpgradeLevel(upgradeTypeEnum, 1);
            Debug.Log($"Upgraded {upgradeType} by 1.");
        }
        else 
        {
            Debug.Log($"Can't parse {upgradeType} to UpgradeType.");
        }
    }
}
