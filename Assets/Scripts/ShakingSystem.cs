using UnityEngine;
using PrimeTween;

public class ShakingSystem : MonoBehaviour
{
    [Header("Shaking Settings")]
    [SerializeField] private float _strengthFactor = 1f;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _frequency = 10f;
    private Camera _camera;
    private Player _player;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        _player = ServicesStorage.Instance.Get<Player>();
        _player.ShakingCam += ShakingCamera;
    }

    void OnDisable()
    {
        _player.ShakingCam -= ShakingCamera;
    }

    void ShakingCamera()
    {
        Tween.ShakeCamera(_camera, _strengthFactor, _duration, _frequency);
    }
}
