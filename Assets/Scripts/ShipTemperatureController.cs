    using UnityEngine;

public class ShipTemperatureController : MonoBehaviour
{
    public float increaseRate = 2f; // �������� ��������� �����������
    public float decreaseRate = 0.5f; // �������� �������� �����������
    public float minTemperature = 0f; // ̳������� �����������
    public float maxTemperature = 120f; // ����������� �����������
    private float currentTemperature = 0f; // ��������� �����������

    private Rigidbody2D rb; // Rigidbody ���������� ��� ���������� ����
    public ScreenRedEffect screenRedEffect; // ��������� �� ������ ScreenRedEffect

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        if (rb.velocity.y > 0)
        {
            IncreaseTemperature();
        }
        else if (rb.velocity.y < 0)
        {
            DecreaseTemperature();
        }

        screenRedEffect.UpdateOverlayTransparency(currentTemperature);
    }

    void IncreaseTemperature()
    {
        currentTemperature += increaseRate * Time.deltaTime;
        currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
        Debug.Log("Temperature increased: " + currentTemperature);
    }

    void DecreaseTemperature()
    {
        currentTemperature -= decreaseRate * Time.deltaTime;
        currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
        Debug.Log("Temperature decreased: " + currentTemperature);
    }
}
