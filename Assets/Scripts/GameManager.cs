using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator _gameOverAnimator;
    [SerializeField] private Button _inverseDirectionButton;
    [Header("Pause Settings")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Sprite _pauseButtonOn;
    [SerializeField] private Sprite _pauseButtonOff;
    [SerializeField] private GameObject _pausePanel;
    public event Action<bool> OnChangePauseState;
    public event Action OnGameOver;
    public bool IsPaused { get; private set; }
    private AudioManager _audioManager;

    void Awake()
    {
        ServicesStorage.Instance.Register(this);
        _pauseButton.onClick.AddListener(() => HandlePressedPauseButton());
        OnGameOver += () => {
            _gameOverAnimator.SetBool("gameOver", true);
            ChangePauseMode(true);
            _audioManager.GameOverSound();
            _pauseButton.interactable = false;
        };
    }

    void HandlePressedPauseButton() 
    {
        IsPaused = !IsPaused;
        _inverseDirectionButton.interactable = !IsPaused;
        _pauseButton.image.sprite = IsPaused ? _pauseButtonOn : _pauseButtonOff;
        _pausePanel?.SetActive(IsPaused);
        ChangePauseMode(IsPaused);
    }

    void OnDestroy()
    {
        _pauseButton.onClick.RemoveAllListeners();
        OnGameOver = null;
    }

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
