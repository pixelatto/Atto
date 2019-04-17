using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerprefsStorageProvider : IStorageService
{

    public string ReadFromStorage(DataChannel channel)
    {
        return PlayerPrefs.GetString(channel.uri);
    }

    public void WriteToStorage(string content, DataChannel channel)
    {
        PlayerPrefs.SetString(channel.uri, content);
    }
}
