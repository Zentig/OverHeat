using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIArmor : MonoBehaviour
{
    [SerializeField] private Armor _armorReference;
    [SerializeField] private Player _playerReference;
    [SerializeField] private Image _armorFillImage;
    [SerializeField] private Text _armorPercentText;
    [SerializeField] private Text _totalhitsTakenText;

    void OnEnable()
    {
        _armorReference.OnArmorChanged += UpdateArmorUI;
        _playerReference.OnPlayerDamaged += UpdateTakenDamage;

    }

    void OnDisable()
    {
        _armorReference.OnArmorChanged -= UpdateArmorUI;
        _playerReference.OnPlayerDamaged -= UpdateTakenDamage;
    }
    
    void UpdateTakenDamage(float totaltakendamage)
    {
     _totalhitsTakenText.text = totaltakendamage.ToString() + "\n hits taken";
    }

    void UpdateArmorUI(int newArmorValue)
    {
        _armorFillImage.fillAmount = (float)newArmorValue / _armorReference.MaxArmor;
        
        float percent =  ((float)newArmorValue / _armorReference.MaxArmor) * 100;
        percent = Mathf.Clamp(percent, 0, 100);       
        
        _armorPercentText.text = percent.ToString("F0") + "%";
    }
}
