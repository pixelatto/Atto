//To use this event system, add event names to the "On" namespace
/*
public namespace On
{
    [System.Serializable] public struct PlayerSpawned { public FfPlayer player; }
    [System.Serializable] public struct PlayerHeroSpawned { public FfPlayer player; public FfActor hero; }
    [System.Serializable] public struct UnitSpawned { public FfActor unit; }
    [System.Serializable] public struct UnitRemoved { public FfActor unit; }
    [System.Serializable] public struct GameSceneLoaded { public List<FfPlayerManifest> playersManifest; public List<FfFactionManifest> factionsManifest; }
}
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class Events : Singleton<Events>
{
    private Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();

    delegate void EventCallback(object data);

    public static void Subscribe<T>(Action<T> callback) where T : struct
    {
        var type = typeof(T);
        if (instance.eventDictionary.TryGetValue(type, out Delegate d))
        {
            instance.eventDictionary[type] = Delegate.Combine(d, callback);
        }
        else
        {
            instance.eventDictionary[type] = callback;
        }
    }

    public static void Trigger<T>(T eventData) where T : struct
    {
        var type = typeof(T);
        if (instance.eventDictionary.TryGetValue(type, out Delegate d))
        {
            (d as Action<T>)?.Invoke(eventData);
        }
    }

}

