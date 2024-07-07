using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreTextUI;
    public int Score { get; private set; }
    public event Action<int> OnScoreChanged;

    void Start()
    {
        ServicesStorage.Instance.Register(this);
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
}
