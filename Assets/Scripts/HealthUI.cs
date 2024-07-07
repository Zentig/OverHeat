using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health _healthReference;
    [SerializeField] private Image _hpPrefab;
    [SerializeField] private Transform _hpParent;
    private GameObjectPool<Image> _pool;

    private void Awake() 
    {
        _healthReference.OnHPChanged += UpdateHPUI;
    }
    
    private void Start() 
    {
        _pool = new GameObjectPool<Image>(_hpPrefab, () => Instantiate(_hpPrefab, _hpParent), default, default, 5, _hpParent);
        _pool.StartPreload();
        UpdateHPUI(_healthReference.HP);
    }

    public void UpdateHPUI(int currentHP) 
    { 
        int activeImages = _pool.ActiveObjects.Count;

        if (currentHP < activeImages) 
        {
            int disableImagesCount = activeImages - currentHP;
            for (int i = 0; i < disableImagesCount; i++)
            {
                _pool.Return(_pool.ActiveObjects[^(i+1)]);
            }
        }
        if (currentHP > activeImages && currentHP <= _healthReference.MaxHealth) 
        {
            int enableImagesCount = currentHP - activeImages;
            for (int i = 0; i < enableImagesCount; i++)
            {
                _pool.Get();
            }
        }
    }
}
