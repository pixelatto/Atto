using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using RSG.Promises;
using System;

public class Bootstrap : MonoBehaviour
{
    public ISerializationService serialization;

    void Start ()
    {
        Core.Bind<IDataChannelService, UriDataChannelProvider>();
        Core.Bind<ILogService, UnityConsoleLogProvider>();
        Core.Bind<ISerializationService, JsonSerializationProvider>();
        Core.Bind<IStorageService, FileStorageProvider>();
        Core.Bind<IAchievementService<DummyAchievement>, DummyAchievementProvider>();
        Core.Bind<IDataBaseService, BinaryDatabaseProvider>();
        Core.Bind<IEventService, BasicEventProvider>();
    }
}
