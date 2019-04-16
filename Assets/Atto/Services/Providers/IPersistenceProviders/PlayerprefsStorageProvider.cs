using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerprefsStorageProvider : IStorageService
{

    string dataPath = "";

    public void SetDestination(string path)
    {
        dataPath = path;
    }

    public string ReadFromStorage()
    {
        return PlayerPrefs.GetString(dataPath);
    }

    public void WriteToStorage(string content)
    {
        PlayerPrefs.SetString(dataPath, content);
    }
}
