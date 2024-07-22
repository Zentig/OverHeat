using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour, IDataPersistence
{
    public int Money { get; private set; }
    public event Action<int> OnMoneyChanged;
    
    private void Start() 
    {
        ServicesStorage.Instance.Register(this);
    }

    public void AddMoney(int money) 
    {
        Money += money;
        OnMoneyChanged?.Invoke(Money);
    }

    public void RemoveMoney(int money) 
    {
        Money -= money;
        OnMoneyChanged?.Invoke(Money);
    }

    public void LoadData(GameData data)
    {
        Money = data.Money;
        OnMoneyChanged?.Invoke(Money);
    }

    public void SaveData(ref GameData data)
    {
        data.Money = Money;
    }
}
