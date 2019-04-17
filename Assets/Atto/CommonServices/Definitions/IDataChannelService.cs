using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataChannelService
{

    DataChannel GetChannel(DataChannelTypes channel);

}

public class DataChannel
{
    DataChannelTypes channelType;
    public string uri;
}

public enum DataChannelTypes { Undefined, Database, Options }