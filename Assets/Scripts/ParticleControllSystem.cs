using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ParticleControllSystem : MonoBehaviour
{
    [SerializeField] public Transform _pointOfSmoke;
    [SerializeField] private ShipTemperatureController _shipTemperatureController;
    [SerializeField] public ParticleSystem _parametersOfParticle;
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = _pointOfSmoke.position;

        var emission = _parametersOfParticle.emission;
        var main = _parametersOfParticle.main;

        switch (_shipTemperatureController.currentTemperature)
        {
            case < 20:
                emission.rateOverTime = 1.0f;
                main.startColor = ParseColor("#FFFFFF");
                break;
            case < 40:
                emission.rateOverTime = 2.0f;
                main.startColor = ParseColor("#E7E7E7");
                break;
            case < 60:
                emission.rateOverTime = 4.0f;
                main.startColor = ParseColor("#CDCDCD");
                break;
            case < 80:
                emission.rateOverTime = 5.0f;
                main.startColor = ParseColor("#B3B3B3");
                break;
            case < 85:
                emission.rateOverTime = 6.0f;
                main.startColor = ParseColor("#9A9A9A");
                break;
            case < 90:
                emission.rateOverTime = 7.0f;
                main.startColor = ParseColor("#8C8C8C");
                break;
            case < 95:
                emission.rateOverTime = 8.0f;
                main.startColor = ParseColor("#808080");
                break;
            case < 100:
                emission.rateOverTime = 9.0f;
                main.startColor = ParseColor("#797979");
                break;
        }


        //emission.rateOverTime = 10.0f;


    }

    private ParticleSystem.MinMaxGradient ParseColor(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            return new ParticleSystem.MinMaxGradient(color);
        }
        else
        {
            return new ParticleSystem.MinMaxGradient(Color.white);  
        }
    }
}
