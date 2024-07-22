using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Buff
{
    public event Action<Coin> OnDestroyed;
    [SerializeField] private int _worth;
    private MoneyManager _moneyManager;

    public override void Init()
    {
        base.Init();
        _moneyManager = ServicesStorage.Instance.Get<MoneyManager>();
    }

    public override void OnCollisionWithPlayer(Player player)
    {
        _moneyManager.AddMoney(_worth);
    }

    protected override void HandleDestroyed()
    {
        OnDestroyed?.Invoke(this);
    }
}
