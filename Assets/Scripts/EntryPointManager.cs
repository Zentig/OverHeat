using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPointManager : MonoBehaviour
{
    private void Awake() 
    {
        GameObject servicesStorage = new GameObject("ServicesStorage");
        servicesStorage.AddComponent<ServicesStorage>();

        GameObject saveLoadManager = new GameObject("SaveLoadManager");
        saveLoadManager.AddComponent<SaveLoadManager>();

        SceneManager.LoadScene(1);
    }
}
