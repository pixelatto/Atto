using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventExtensions
{
    public static void InvokeIfNotNull(this UnityEvent e)
    {
        if (e != null)
        {
            e.Invoke();
        }
    }

    public static void InvokeIfNotNull(this Action action)
    {
        if (action != null)
        {
            action.Invoke();
        }
    }

    public static void InvokeIfNotNull<T>(this Action<T> action, T obj)
    {
        if (action != null)
        {
            action.Invoke(obj);
        }
    }
}