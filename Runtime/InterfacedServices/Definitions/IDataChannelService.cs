using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataChannelService
{
    /// <summary>
    /// Get a channel from some Id.
    /// </summary>
    /// <param name="channelId">The channel Id: either the name, a DataChannelType enum value or the int Id</param>
    /// <returns>The corresponding data channel for I/O operations</returns>
    DataChannel GetChannel(object channelId);

    List<int> GetAvailableChannels();

}

[System.Serializable]
public class DataChannel
{
    public string channelName;
    public int channelId;
    public string uri;
}
