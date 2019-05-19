using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[BindService]
public class UriDataChannelProvider : IDataChannelService
{

    Dictionary<int, DataChannel> channelDictionary;

    ISettingsService settings;
    ILogService logger;

    public UriDataChannelProvider()
    {
        logger = Atto.Get<ILogService>();
        settings = Atto.Get<ISettingsService>();

        if (settings.Current != null)
        {
            string storagePath = settings.Current.storagePath;
            if (storagePath.Contains("dataPath"))
            {
                storagePath = storagePath.Replace("dataPath", Application.dataPath);
            } else if (storagePath.Contains("persistentDataPath"))
            {
                storagePath = storagePath.Replace("persistentDataPath", Application.persistentDataPath);
            }
            var dataChannels = settings.Current.dataChannels;
            foreach (var channel in dataChannels)
            {
                if (channel != null && channel.channelId != 0 && channel.uri != "")
                {
                    var fullPath = channel.uri;
                    if (channel.uri.StartsWith("/"))
                    {
                        fullPath = storagePath + channel.uri;
                    }
                    var newDataChannel = new DataChannel() { uri = fullPath, channelId = channel.channelId };
                    AddEntry(channel.channelId, newDataChannel);
                }
            }
        }
    }

    void AddEntry(int channeId, DataChannel entry)
    {
        if (channelDictionary == null)
        {
            channelDictionary = new Dictionary<int, DataChannel>();
        }
        channelDictionary.Add(channeId, entry);
    }

    public DataChannel GetChannel(object channelId)
    {
        if (channelId is int || channelId.GetType().IsEnum)
        {
            int intId = (int)channelId;
            if (channelDictionary.ContainsKey(intId))
            {
                return channelDictionary[intId];
            }
            else
            {
                logger.Error("Unregistered channel: " + channelId.ToString());
                return null;
            }
        }
        else if (channelId is string)
        {
            string stringId = (string)channelId;
            return GetChannelByName(stringId);
        }
        else
        {
            return null;
        }
    }

    DataChannel GetChannelByName(string channelName)
    {
        foreach (var channelEntry in channelDictionary)
        {
            if (channelEntry.Value.channelName == channelName)
            {
                return channelEntry.Value;
            }
        }
        return null;
    }

    public List<int> GetAvailableChannels()
    {
        var result = new List<int>();
        foreach (var channelEntry in channelDictionary)
        {
            result.Add(channelEntry.Key);
            logger.Log(channelEntry.Key.ToString() + " -> " + channelEntry.Value.uri);
        }
        return result;
    }
}
