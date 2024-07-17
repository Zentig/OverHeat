using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class SaveLoadManager : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] private string _fileName = "data.json";
    [SerializeField] private bool _useEncryption = false;
    private FileDataHandler _fileDataHandler;
    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;

    private void Awake() 
    {
        ServicesStorage.Instance.Register(this);
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
    }

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        _dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene) => SaveGame();

    private void OnApplicationQuit() => SaveGame();

    public void LoadGame() 
    {
        _gameData = _fileDataHandler.Load();

        if (_gameData == null) 
        {
            Debug.Log("GameData is null. Initializing default values.");
            _gameData = new GameData();
        }

        foreach (var dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gameData);
        }
    }

    public void SaveGame() 
    {
        foreach (var dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref _gameData);
        }

        _fileDataHandler.Save(_gameData);
    }
}

[Serializable]
public class GameData 
{
    public int BestScore;
    public float SfxVolume;
    public float MusicVolume;
    public Dictionary<UpgradeTypes, int> Upgrades;

    public GameData()
    {
        BestScore = 0;
        SfxVolume = 1;
        MusicVolume = 1;
        Upgrades = new Dictionary<UpgradeTypes, int>();

        foreach (UpgradeTypes key in Enum.GetValues(typeof(UpgradeTypes))) 
        {
            Upgrades.Add(key, default);
        }
    }
}