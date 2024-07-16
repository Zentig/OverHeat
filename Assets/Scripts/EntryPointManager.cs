using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPointManager : MonoBehaviour
{
    private void Awake() 
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(1);
    }
}
