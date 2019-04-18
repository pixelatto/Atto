using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UriDataChannelProvider : IDataChannelService
{

    Dictionary<DataChannelTypes, DataChannel> channelDictionary;

    public UriDataChannelProvider()
    {
        if (Atto.Settings.Current != null)
        {
            string basePath = Atto.Settings.Current.basePath;
            if (basePath == "Application.dataPath")
            {
                basePath = Application.dataPath;
            } else if (basePath == "Application.persistentDataPath")
            {
                basePath = Application.persistentDataPath;
            }
            var dataChannels = Atto.Settings.Current.dataChannels;
            foreach (var channel in dataChannels)
            {
                if (channel != null && channel.type != DataChannelTypes.Undefined && channel.uri != "")
                {
                    var newDataChannel = new DataChannel() { uri = basePath + channel.uri, type = channel.type };
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
            Atto.Logger.Error("Unregistered channel: " + channelType.ToString());
            return null;
        }
    }

    public List<DataChannelTypes> GetAvailableChannels()
    {
        var result = new List<DataChannelTypes>();
        foreach (var channelEntry in channelDictionary)
        {
            result.Add(channelEntry.Key);
            Atto.Logger.Log(channelEntry.Key.ToString() + " -> " + channelEntry.Value.uri);
        }
        return result;
    }
}
