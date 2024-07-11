using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPointManager : MonoBehaviour
{
    private void Awake() 
    {
        GameObject go = new GameObject("ServicesStorage");
        go.AddComponent<ServicesStorage>();
    }
    void Start() => SceneManager.LoadScene(0);
}
