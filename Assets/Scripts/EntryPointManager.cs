using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPointManager : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(this);
    private void Start() => SceneManager.LoadScene(1);
}
