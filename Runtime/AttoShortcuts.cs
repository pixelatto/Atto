using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static partial class Atto
{

    public static IDataChannelService Channels
    {
        get { return container.Get<IDataChannelService>(); }
    }

    public static ILogService Logger
    {
        get { return container.Get<ILogService>(); }
    }

    public static ISerializationService Serialization
    {
        get { return container.Get<ISerializationService>(); }
    }

    public static IStorageService Storage
    {
        get { return container.Get<IStorageService>(); }
    }

    public static IEventService Event
    {
        get { return container.Get<IEventService>(); }
    }

    public static ISceneService Scenes
    {
        get { return container.Get<ISceneService>(); }
    }

    public static IDataBaseService Database
    {
        get { return container.Get<IDataBaseService>(); }
    }

    public static IInputService Input
    {
        get { return container.Get<IInputService>(); }
    }
}