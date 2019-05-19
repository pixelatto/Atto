using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataChannelService
{
    /// <summary>
    /// Get a channel by its Id. A helper enum "DataChannelTypes" is generated. Use this enum casting to (int) as argument
    /// </summary>
    /// <param name="channelId">The channel Id</param>
    /// <returns>The corresponding data channel for I/O operations</returns>
    DataChannel GetChannel(int channelId);
    DataChannel GetChannelByName(string channelName);
    List<int> GetAvailableChannels();

}

[System.Serializable]
public class DataChannel
{
    public string channelName;
    public int channelId;
    public string uri;
}
