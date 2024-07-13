using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator _gameOverAnimator;
    [SerializeField] private Button _inverseDirectionButton;
    [SerializeField] private Button _pauseButton;
    public event Action<bool> OnChangePauseState;
    public event Action OnGameOver;
    public bool IsPaused { get; private set; }
    private AudioManager _audioManager;

    void Awake()
    {
        ServicesStorage.Instance.Register(this);
        OnChangePauseState += (state) => _inverseDirectionButton.interactable = !state;
        OnGameOver += () => {
            _gameOverAnimator.SetBool("gameOver", true);
            ChangePauseMode(true);
            _audioManager.GameOverSound();
            _pauseButton.interactable = false;
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

    public void ChangePauseMode() 
    {
        IsPaused = !IsPaused;
        OnChangePauseState?.Invoke(IsPaused);
    }

    public void GameOver() => OnGameOver?.Invoke();
}
