using UnityEngine;
using PrimeTween;

public class ShakingSystem : MonoBehaviour
{
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
        Debug.Log("Камера спіймала потрясіння!");
        Tween.ShakeCamera(_camera, strengthFactor: 1.0f, duration: 0.5f, frequency: 10);
    }
}
