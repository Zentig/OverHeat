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

    private void OnEnable() 
    {
        ServicesStorage.Instance.Register(this);
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _scoreManager = ServicesStorage.Instance.Get<ScoreManager>();
        _scoreManager.OnScoreChanged += HandleScoreChanged;
    }

    public void PlayGameOver() 
    {
        _animator.SetBool("gameOver", true);
    }

    private void HandleScoreChanged(int score)
    {
        _gameOverScoreText.text = "Your score: " + score.ToString();
    }

    public void Restart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToWorkshop() 
    {
        SceneManager.LoadScene(1);
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
