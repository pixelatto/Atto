using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[BindService] -> This service should not autobind, since it's initialized before anything else
public class AttoSettingsProvider : ISettingsService
{
    public AttoSettings Current { get; private set; }

    string settingsPath { get { return Application.dataPath + "/attoSettings.json"; } }

    AttoSettings defaultSettings = new AttoSettings()
    {
        coreName = "Core",
        autoBindCommonServices = true,
        storagePath = "dataPath",
        dataChannels = new List<DataChannel>()
        {
            new DataChannel() { type = DataChannelTypes.Database, uri = "/Data.sav" },
            new DataChannel() { type = DataChannelTypes.Options, uri = "/Options.sav" },
            new DataChannel() { type = DataChannelTypes.SavedGame, uri = "/Save.sav" },
            new DataChannel() { type = DataChannelTypes.Rankings, uri = "/Rankings.sav"}
        }
    };

    public AttoSettingsProvider()
    {
        if (File.Exists(settingsPath))
        {
            var settingsText = File.ReadAllText(settingsPath);
            AttoSettings parsedSettings = JsonUtility.FromJson<AttoSettings>(settingsText);
            Current = parsedSettings;
        }
        else
        {
            Current = defaultSettings;
            string serializedSettings = JsonUtility.ToJson(Current, true);
            File.WriteAllText(settingsPath, serializedSettings);
        }
    }

    public void SaveSettings()
    {
        string serializedSettings = JsonUtility.ToJson(Current);
        File.WriteAllText(settingsPath, serializedSettings);
    }
}
