    using UnityEngine;

public class ShipTemperatureController : MonoBehaviour
{
    public float increaseRate = 2f; // Швидкість підвищення температури
    public float decreaseRate = 0.5f; // Швидкість зниження температури
    public float minTemperature = 0f; // Мінімальна температура
    public float maxTemperature = 120f; // Максимальна температура
    private float currentTemperature = 0f; // Початкова температура

    private Rigidbody2D rb; // Rigidbody компонента для відстеження руху
    public ScreenRedEffect screenRedEffect; // Посилання на скрипт ScreenRedEffect

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
