using UnityEngine;
using UnityEngine.UI;

public class ScreenRedEffect : MonoBehaviour
{
    public Image redOverlay;
    public float maxTemperature = 120f; // �����������, ��� ��� ����� ���� �������� ��������
    public float startRedEffectTemperature = 40f; // �����������, ��� ��� ���������� ���������� ������

    // ����� ��� ��������� ��������� ���������� ������� �� ����������� �������
    public void UpdateOverlayTransparency(float shipTemperature)
    {
        // ���������� ��������� ������� �� �����������
        float alpha = 0f;
        if (shipTemperature > startRedEffectTemperature)
        {
            alpha = Mathf.Clamp01((shipTemperature - startRedEffectTemperature) / (maxTemperature - startRedEffectTemperature));
        }

        // ����������� ��������� �� ����������
        redOverlay.color = new Color(1f, 0f, 0f, alpha/2);
    }
}
