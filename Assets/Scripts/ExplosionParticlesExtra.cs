using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticlesExtra : MonoBehaviour
{
    private Animator _gameOverAnimator;

    void Start() => _gameOverAnimator = ServicesStorage.Instance.Get<Animator>();

    public void PlayGameOver() => _gameOverAnimator.SetBool("gameOver", true);
}
