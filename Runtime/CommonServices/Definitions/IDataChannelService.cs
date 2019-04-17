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

public enum DataChannelTypes {
    Undefined, Database, Options, SavedGame, Rankings,
    Custom_0, Custom_1, Custom_2, Custom_3, Custom_4, Custom_5, Custom_6, Custom_7, Custom_8, Custom_9
}