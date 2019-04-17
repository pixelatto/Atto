using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UriDataChannelProvider : IDataChannelService
{

    Dictionary<DataChannelTypes, DataChannel> pathDictionary = new Dictionary<DataChannelTypes, DataChannel>();

    public UriDataChannelProvider()
    {
        //TODO: If a DataChannels.js file exists, build this entries from there
        string basePath = Application.dataPath;
        AddEntry(DataChannelTypes.Database , basePath + "/Data.sav");
        AddEntry(DataChannelTypes.Options  , basePath + "/Options.sav");
        AddEntry(DataChannelTypes.Rankings , basePath + "/Rankings.sav");
        AddEntry(DataChannelTypes.SavedGame, basePath + "/SavedGame.sav");
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
            Atto.Logger.Error("Unknown path entry.");
            return null;
        }
    }

}
