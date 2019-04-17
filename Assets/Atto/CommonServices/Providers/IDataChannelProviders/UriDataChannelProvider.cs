using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UriDataChannelProvider : IDataChannelService
{

    Dictionary<DataChannelTypes, DataChannel> pathDictionary = new Dictionary<DataChannelTypes, DataChannel>();

    public UriDataChannelProvider()
    {
        string basePath = Application.dataPath;
        AddEntry(DataChannelTypes.Database, basePath + "/Data.sav");
        AddEntry(DataChannelTypes.Options,  basePath + "/Options.sav");
    }

    void AddEntry(DataChannelTypes key, string entry)
    {
        pathDictionary.Add(key, new DataChannel() { uri = entry });
    }

    public DataChannel GetChannel(DataChannelTypes channelType)
    {
        if (pathDictionary.ContainsKey(channelType))
        {
            return pathDictionary[channelType];
        }
        else
        {
            Core.Logger.Error("Unknown path entry.");
            return null;
        }
    }

}
