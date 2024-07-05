using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<Image> _hpImages;
    [SerializeField] private Health _healthReference;
    [SerializeField] private Image _hpPrefab;
    [SerializeField] private Transform _hpParent;
    
    private void Awake() 
    {
        _healthReference.OnHPChanged += UpdateHPUI;
    }
    
    private void Start() 
    {
        for (int i = 0; i < _healthReference.MaxHealth; i++) 
        {
            Image newHPImage = Instantiate(_hpPrefab, _hpParent);
            _hpImages.Add(newHPImage);
        }
        UpdateHPUI(_healthReference.HP);
    }

    public void UpdateHPUI(int currentHP) 
    { 
        int activeImages = _hpImages.FindAll(x => x.gameObject.activeInHierarchy).Count;

        if (currentHP < activeImages) 
        {
            int disableImagesCount = _healthReference.MaxHealth - currentHP;
            for (int i = 0; i < disableImagesCount; i++)
            {
                _hpImages[^(i+1)].gameObject.SetActive(false);
            }
        }
        else if (currentHP > activeImages) 
        {
            int enableImagesCount = _healthReference.MaxHealth - currentHP;
            for (int i = enableImagesCount; i > 0; i--)
            {
                _hpImages[^(i+1)].gameObject.SetActive(true);
            }
        }
    }
}
