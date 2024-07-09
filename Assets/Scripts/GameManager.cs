using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator _gameOverAnimator;
    public event Action<bool> OnChangePauseState;
    public event Action OnGameOver;
    public bool IsPaused { get; private set; }
    private AudioManager _audioManager;

    void Awake()
    {
        ServicesStorage.Instance.Register(this);
        OnGameOver += () => {
            _gameOverAnimator.SetBool("gameOver", true);
            ChangePauseMode(true);
            _audioManager.GameOverSound();
        };
    }

    void OnDestroy() => OnGameOver = null;

    void Start() 
    {
        ChangePauseMode(false);
        Application.targetFrameRate = 45;
        _audioManager = ServicesStorage.Instance.Get<AudioManager>();
    }

    public void ChangePauseMode(bool isPaused) 
    {
        IsPaused = isPaused;
        OnChangePauseState?.Invoke(isPaused);
    }

    public void GameOver() => OnGameOver?.Invoke();
}
