using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerprefsPersistenceProvider : IPersistenceService<string>
{

    string dataPath = "";

    public void SetDestination(string path)
    {
        dataPath = path;
    }

    public void Append(string content)
    {
        var currentData = PlayerPrefs.GetString(dataPath);
        var newData = currentData + content;
        PlayerPrefs.SetString(dataPath, newData);
    }

    public string Read()
    {
        return PlayerPrefs.GetString(dataPath);
    }

    public void Write(string content)
    {
        PlayerPrefs.SetString(dataPath, content);
    }
}
