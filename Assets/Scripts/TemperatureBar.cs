using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureBar : MonoBehaviour
{
    [SerializeField] private Slider _temperatureSlider;
    [SerializeField] private int _initialTemperature;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private bool _upTemperature;
    [SerializeField] private int _changeNumber = 1;
    private int _currentTemperature;

    void Start()
    {
        _currentTemperature = _initialTemperature;
    }

    private void FixedUpdate()
    {
        _temperatureSlider.value = _currentTemperature;

        _changeNumber = _playerMovement.IsGoingUp ? 1 : -1;

        ChangeTemperature();
    }

    void ChangeTemperature()
    {
        _currentTemperature += _changeNumber;
        _currentTemperature = Mathf.Clamp(_currentTemperature, 0, 100);
        _temperatureSlider.value = _currentTemperature;
        Debug.Log(_currentTemperature);
    }
    void Update()
    {
        
    }
}
