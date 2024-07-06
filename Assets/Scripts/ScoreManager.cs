using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreTextUI;
    public int Score { get; private set; }

    void Awake()
    {
        ServicesStorage.Instance.Register(this);
    }

    public void AddScore(int score) 
    {
        Score += score;
        _scoreTextUI.text = Score.ToString();
    }

    public void RemoveScore(int score) 
    {
        Score -= score;
        _scoreTextUI.text = Score.ToString();
    }
}
