using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ParticleControllSystem : MonoBehaviour
{
    [SerializeField] public Transform _pointOfSmoke;
    [SerializeField] private ShipTemperatureController _shipTemperatureController;
    [SerializeField] public List<ParticleSystem> _parametersOfParticle;
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = _pointOfSmoke.position;

        float rateOverTime = default;
        string main = "";

        switch (_shipTemperatureController.currentTemperature)
        {
            case < 20:
                rateOverTime = 1.0f;
                main = "#FFFFFF";
                break;
            case < 40:
                rateOverTime = 2.0f;
                main = "#E7E7E7";
                break;
            case < 60:
                rateOverTime = 4.0f;
                main = "#CDCDCD";
                break;
            case < 80:
                rateOverTime = 5.0f;
                main = "#B3B3B3";
                break;
            case < 85:
                rateOverTime = 6.0f;
                main = "#9A9A9A";
                break;
            case < 90:
                rateOverTime = 7.0f;
                main = "#8C8C8C";
                break;
            case < 95:
                rateOverTime = 8.0f;
                main = "#808080";
                break;
            case < 100:
                rateOverTime = 9.0f;
                main = "#797979";
                break;
        }

        foreach (var item in _parametersOfParticle)
        {
            var emission = item.emission;
            emission.rateOverTime = rateOverTime;
            
            var nextMain = item.main;
            nextMain.startColor = ParseColor(main);
        }
    }

    private ParticleSystem.MinMaxGradient ParseColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            return new ParticleSystem.MinMaxGradient(color);
        }
        else
        {
            return new ParticleSystem.MinMaxGradient(Color.white);
        }
    }
}
