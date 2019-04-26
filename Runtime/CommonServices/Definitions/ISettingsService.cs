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
    public List<DataChannel> dataChannels;
    public bool autoBindCommonServices;
}
