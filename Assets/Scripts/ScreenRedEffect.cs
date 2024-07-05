using UnityEngine;
using UnityEngine.UI;

public class ScreenRedEffect : MonoBehaviour
{
    public Image redOverlay;
    public float maxTemperature = 120f; // Температура, при якій екран буде повністю червоним
    public float startRedEffectTemperature = 40f; // Температура, при якій починається червоніння екрану

    // Метод для оновлення прозорості зображення залежно від температури корабля
    public void UpdateOverlayTransparency(float shipTemperature)
    {
        // Обчислюємо прозорість залежно від температури
        float alpha = 0f;
        if (shipTemperature > startRedEffectTemperature)
        {
            alpha = Mathf.Clamp01((shipTemperature - startRedEffectTemperature) / (maxTemperature - startRedEffectTemperature));
        }

        // Застосовуємо прозорість до зображення
        redOverlay.color = new Color(1f, 0f, 0f, alpha);
    }
}
