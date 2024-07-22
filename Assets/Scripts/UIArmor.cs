using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArmor : MonoBehaviour
{
    private Armor _armorReference;
    [SerializeField] private Image armorFillImage;
    [SerializeField] private float fillAmount;
    
    void OnEnable()
    {
        _armorReference.OnArmorChanged += UpdateArmorUI; 
    }
    void Update()
    {
        
    }
    void UpdateArmorUI(int newArmorValue = 1)
    {

        fillAmount = ((float)newArmorValue / _armorReference.MaxArmor) * _armorReference.CurrentArmor;
       
        armorFillImage.fillAmount = fillAmount;
    }

}
