using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureBar : MonoBehaviour
{
    [SerializeField] private ShipTemperatureController _shipTemperatureController;
    [SerializeField] private Slider _temperatureSlider;

    private void FixedUpdate()
    {
        _temperatureSlider.value = _shipTemperatureController.currentTemperature;
    }
    
    }
