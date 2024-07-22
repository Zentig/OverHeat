using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIArmor : MonoBehaviour
{
    [SerializeField] private Armor _armorReference;
    [SerializeField] private Image _armorFillImage;
    [SerializeField] private Text _armorPercentText;

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
        float percent =  ((float)newArmorValue / _armorReference.MaxArmor) * 100;
        percent = Mathf.Clamp(percent, 0, 100);       
        
        _armorPercentText.text = percent.ToString("F0") + "%";
    }
}
