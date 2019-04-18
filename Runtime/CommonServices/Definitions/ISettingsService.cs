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
    public string basePath;
    public List<DataChannel> dataChannels;
    public bool autoBindCommonServices;
}
