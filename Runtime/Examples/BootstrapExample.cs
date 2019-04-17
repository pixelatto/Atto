using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using RSG.Promises;
using System;

public class BootstrapExample : MonoBehaviour
{

    void Awake()
    {
        Atto.Bind<IDataChannelService, UriDataChannelProvider>();
        Atto.Bind<ILogService, UnityConsoleLogProvider>();
        Atto.Bind<ISerializationService, JsonSerializationProvider>();
        Atto.Bind<IStorageService, FileStorageProvider>();
        Atto.Bind<IAchievementService, SimpleAchievementProvider>();
        Atto.Bind<IDataBaseService, BinaryDatabaseProvider>();
        Atto.Bind<IEventService, SimpleEventProvider>();
        Atto.Bind<IInputService, SimpleInputProvider>();
    }

}