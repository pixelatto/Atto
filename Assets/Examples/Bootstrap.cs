using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using RSG.Promises;
using System;

public class Bootstrap : MonoBehaviour
{

    void Awake()
    {
        Core.Bind<IDataChannelService, UriDataChannelProvider>();
        Core.Bind<ILogService, UnityConsoleLogProvider>();
        Core.Bind<ISerializationService, JsonSerializationProvider>();
        Core.Bind<IStorageService, FileStorageProvider>();
        Core.Bind<IAchievementService, SimpleAchievementProvider>();
        Core.Bind<IDataBaseService, BinaryDatabaseProvider>();
        Core.Bind<IEventService, SimpleEventProvider>();
        Core.Bind<IInputService, SimpleInputProvider>();
    }

}
