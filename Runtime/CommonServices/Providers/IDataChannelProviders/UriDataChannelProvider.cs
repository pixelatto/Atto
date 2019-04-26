using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[BindService("Channel")]
public class UriDataChannelProvider : IDataChannelService
{

    Dictionary<DataChannelTypes, DataChannel> channelDictionary;

    ISettingsService settings;
    ILogService logger;

    public UriDataChannelProvider()
    {
        logger = Atto.Get<ILogService>();
        settings = Atto.Get<ISettingsService>();

        if (settings.Current != null)
        {
            string storagePath = settings.Current.storagePath;
            if (storagePath == "dataPath")
            {
                storagePath = Application.dataPath;
            } else if (storagePath == "persistentDataPath")
            {
                storagePath = Application.persistentDataPath;
            }
            var dataChannels = settings.Current.dataChannels;
            foreach (var channel in dataChannels)
            {
                if (channel != null && channel.type != DataChannelTypes.Undefined && channel.uri != "")
                {
                    var fullPath = channel.uri;
                    if (channel.uri.StartsWith("/"))
                    {
                        fullPath = storagePath + channel.uri;
                    }
                    var newDataChannel = new DataChannel() { uri = fullPath, type = channel.type };
                    AddEntry(channel.type, newDataChannel);
                }
            }
        }
    }

    void AddEntry(DataChannelTypes key, DataChannel entry)
    {
        if (channelDictionary == null)
        {
            channelDictionary = new Dictionary<DataChannelTypes, DataChannel>();
        }
        channelDictionary.Add(key, entry);
    }

    public DataChannel GetChannel(DataChannelTypes channelType)
    {
        if (channelDictionary.ContainsKey(channelType))
        {
            return channelDictionary[channelType];
        }
        else
        {
            logger.Error("Unregistered channel: " + channelType.ToString());
            return null;
        }
    }

    public List<DataChannelTypes> GetAvailableChannels()
    {
        var result = new List<DataChannelTypes>();
        foreach (var channelEntry in channelDictionary)
        {
            result.Add(channelEntry.Key);
            logger.Log(channelEntry.Key.ToString() + " -> " + channelEntry.Value.uri);
        }
        return result;
    }
}
