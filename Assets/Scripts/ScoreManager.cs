using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TextMeshProUGUI _scoreTextUI;
    [SerializeField] private TextMeshProUGUI _bestScoreTextUI;
    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public event Action<int> OnScoreChanged;
    private GameManager _gameManager;

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
    }

    void Start()
    {
        Score = 0;
        _gameManager = ServicesStorage.Instance.Get<GameManager>();
        _gameManager.OnGameOver += UpdateBestScoreText;
    }

    private void UpdateBestScoreText()
    {
        _bestScoreTextUI.text = $"Your best score: {(BestScore < Score ? Score : BestScore)}";
    }

    public void AddScore(int score) 
    {
        Score += score;
        _scoreTextUI.text = Score.ToString();
        OnScoreChanged?.Invoke(Score);
    }

    public void RemoveScore(int score) 
    {
        Score -= score;
        _scoreTextUI.text = Score.ToString();
        OnScoreChanged?.Invoke(Score);
    }

    public void LoadData(GameData data)
    {
        BestScore = data.BestScore;
    }

    public void SaveData(ref GameData data)
    {
        if (data.BestScore < Score) data.BestScore = Score;
    }
}