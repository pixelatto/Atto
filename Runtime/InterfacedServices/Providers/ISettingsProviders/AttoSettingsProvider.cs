using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[BindService]
public class AttoSettingsProvider : ISettingsService
{
    public AttoSettings Current { get; private set; }

    string settingsPath { get { return Application.dataPath + "/Plugins/Atto/AttoSettings.json"; } }

    AttoSettings defaultSettings = new AttoSettings()
    {
        coreName = "Core",
        autoBindCommonServices = true,
        storagePath = "dataPath",
        dataChannels = new List<DataChannel>()
        {
            new DataChannel() { channelName = "Database", channelId = 1, uri = "/Data.sav" },
            new DataChannel() { channelName = "Options", channelId = 2, uri = "/Options.sav" },
            new DataChannel() { channelName = "Save", channelId = 3, uri = "/Save.sav" },
            new DataChannel() { channelName = "Rankings", channelId = 4, uri = "/Rankings.sav"}
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
            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            }
            File.WriteAllText(settingsPath, serializedSettings);
        }
    }

    public void SaveSettings()
    {
        string serializedSettings = JsonUtility.ToJson(Current);
        File.WriteAllText(settingsPath, serializedSettings);
    }
}
