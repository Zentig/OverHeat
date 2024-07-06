    using UnityEngine;

public class ShipTemperatureController : MonoBehaviour
{
    public float increaseRate = 2f; // �������� ��������� �����������
    public float decreaseRate = 0.5f; // �������� �������� �����������
    public float minTemperature = 0f; // ̳�������� �����������
    public float maxTemperature = 120f; // ����������� �����������
    public float currentTemperature = 0f; // ��������� �����������
    private bool _isPaused = false;
    private GameManager _gm;
    private Rigidbody2D rb; // Rigidbody ���������� ��� ���������� ����
    public ScreenRedEffect screenRedEffect; // ��������� �� ������ ScreenRedEffect

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        _gm = ServicesStorage.Instance.Get<GameManager>();
        _gm.OnChangePauseState += HandlePauseState;
    }

    void HandlePauseState(bool state) 
    {
        _isPaused = state;
    }

    void Update()
    {
        screenRedEffect.UpdateOverlayTransparency(currentTemperature);
        
        if (_isPaused && currentTemperature != 0) { 
            currentTemperature = 0;
            return;
        }
        else if (_isPaused) return;

        screenRedEffect.UpdateOverlayTransparency(currentTemperature);

        if (rb.velocity.y > 0)
        {
            IncreaseTemperature();
        }
        else if (rb.velocity.y < 0)
        {
            DecreaseTemperature();
        }
    }

    void IncreaseTemperature()
    {
        currentTemperature += increaseRate * Time.deltaTime;
        currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
       // Debug.Log("Temperature increased: " + currentTemperature);
    }

    void DecreaseTemperature()
    {
        currentTemperature -= decreaseRate * Time.deltaTime;
        currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
      //  Debug.Log("Temperature decreased: " + currentTemperature);
    }
}
