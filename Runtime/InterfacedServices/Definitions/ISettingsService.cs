using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsService
{
    AttoSettings Current { get; }
}

[System.Serializable]
public class AttoSettings
{
    public string coreName = "";
    public string storagePath;
    public string attoAccessPath;
    public List<DataChannel> dataChannels;
    public bool autoBindCommonServices;

    public string GetFullAttoAccessPath()
    {
        var realPath = attoAccessPath;
        realPath = realPath.Replace("[persistentDataPath]", Application.persistentDataPath);
        realPath = realPath.Replace("[dataPath]", Application.dataPath);
        return realPath;
    }
}
