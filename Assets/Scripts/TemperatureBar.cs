using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureBar : MonoBehaviour
{

    [SerializeField] private Slider _temperature;
    [SerializeField] private int _startTemperature;
     private int _currentTemperature;
    [SerializeField] private PlayerMovement _moveUp;
    [SerializeField] private bool _upTemperature;
    [SerializeField] private int _changeNumber = 1;
    void Start()
    {
        _currentTemperature = _startTemperature;
        
    }

    private void FixedUpdate()
    {
           _temperature.value = _currentTemperature;

        _upTemperature = _moveUp._isUp;
        if (_upTemperature )
        {
            _changeNumber = 1;
        }
    else if(!_upTemperature )
        {
            _changeNumber = -1;
        }
        ChangeTemperature();
    }

    void ChangeTemperature()
    {
        _currentTemperature += _changeNumber;
        _temperature.value = _currentTemperature;
        Debug.Log(_currentTemperature);
    }
    void Update()
    {
        
    }
}
