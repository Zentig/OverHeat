using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverScoreText;
    private Animator _animator;
    private ScoreManager _scoreManager;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        ServicesStorage.Instance.Register(_animator);
        _scoreManager = ServicesStorage.Instance.Get<ScoreManager>();
        _scoreManager.OnScoreChanged += HandleScoreChanged;
    }

    private void HandleScoreChanged(int score)
    {
        _gameOverScoreText.text = "Your score: " + score.ToString();
    }

    public void Restart() 
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
