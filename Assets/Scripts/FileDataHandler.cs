using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string _dataDirectoryPath;
    private string _dataFileName;
    private bool _useEncryption;
    private readonly string _encryptionkey = "rPMAfMYaBXuCcittn1tui5e2eTrna42S";

    public FileDataHandler(string dataDirectoryPath, string dataFileName, bool useEncryption = false)
    {
        _dataDirectoryPath = dataDirectoryPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    public GameData Load() 
    {
        string fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);

        GameData loadedData = null;

        if (File.Exists(fullPath)) 
        {
            try 
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open)) 
                {
                    using (StreamReader reader = new StreamReader(stream)) 
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (_useEncryption) dataToLoad = Encryptor.DecryptString(dataToLoad, _encryptionkey);
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception ex) 
            {
                Debug.LogError($"Error occured while trying to load data from file: {fullPath}\n{ex}");
            }
        }

        return loadedData;
    }

    public void Save(GameData data) 
    {
        string fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);
        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);

            if (_useEncryption) dataToStore = Encryptor.EncryptString(dataToStore, _encryptionkey);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception ex) 
        {
            Debug.LogError($"Error occured while trying to save data to file: {fullPath}\n{ex}");
        }
    }
}
