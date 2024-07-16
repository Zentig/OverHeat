using System;
using UnityEngine;

public class ShipTemperatureController : MonoBehaviour
{
    public event Action OnOverheat;
    [field:SerializeField] public float IncreaseRate { get; private set; } = 2f; 
    [field:SerializeField] public float DecreaseRate { get; private set; } = 0.5f; 
    [field:SerializeField] public float MinTemperature { get; private set; } = 0f;  
    [field:SerializeField] public float MaxTemperature { get; private set; } = 120f; 
    [field:SerializeField] public float CurrentTemperature { get; private set; } = 0f; 
    [SerializeField] private ScreenRedEffect _screenRedEffect;
    private bool _isPaused = false;
    private GameManager _gm;
    private Rigidbody2D _rb;
    private bool _isOverheatInvoked;

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); 
        _gm = ServicesStorage.Instance.Get<GameManager>();
        _gm.OnChangePauseState += HandlePauseState;
        _gm.OnGameOver += HandleGameOver;
    }

    private void OnDestroy() 
    {
        _gm.OnChangePauseState -= HandlePauseState;
        _gm.OnGameOver -= HandleGameOver;
    }

    private void HandlePauseState(bool state) => _isPaused = state;

    private void HandleGameOver() => CurrentTemperature = 0;

    void Update()
    {        
        if (_isPaused) return;

        _screenRedEffect.UpdateOverlayTransparency(CurrentTemperature);
        if (CurrentTemperature >= MaxTemperature && !_isOverheatInvoked) { 
            OnOverheat?.Invoke(); 
            _isOverheatInvoked = true;
        }

        switch (_rb.velocity.y)
        {
            case > 0:
                IncreaseTemperature();
                break;
            case < 0:
                DecreaseTemperature();
                break;
        }
    }

    void IncreaseTemperature()
    {
        CurrentTemperature += IncreaseRate * Time.deltaTime;
        CurrentTemperature = Mathf.Clamp(CurrentTemperature, MinTemperature, MaxTemperature);
       // Debug.Log("Temperature increased: " + currentTemperature);
    }

    void DecreaseTemperature()
    {
        CurrentTemperature -= DecreaseRate * Time.deltaTime;
        CurrentTemperature = Mathf.Clamp(CurrentTemperature, MinTemperature, MaxTemperature);
       // Debug.Log("Temperature decreased: " + currentTemperature);
    }

    public void AddTemperature(float value) => CurrentTemperature += value;
}
