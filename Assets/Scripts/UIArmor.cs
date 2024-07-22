using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArmor : MonoBehaviour
{
    [SerializeField] private Armor _armorReference;
    [SerializeField] private Image _armorFillImage;
    
    void OnEnable()
    {
        _armorReference.OnArmorChanged += UpdateArmorUI; 
    }

    void OnDisable()
    {
        _armorReference.OnArmorChanged -= UpdateArmorUI; 
    }

    void UpdateArmorUI(int newArmorValue)
    {
        _armorFillImage.fillAmount = (float)newArmorValue / _armorReference.MaxArmor;
    }
}
