using UnityEngine;

public static class BehaviourExtensions
{
    public static T GetOrAddComponent<T>(this Behaviour target) where T : Component
    {
        T result = target.GetComponent<T>();

        if (result == null)
        {
            result = target.gameObject.AddComponent<T>();
        }
        return result;
    }

    public static T GetOrAddComponent<T>(this GameObject target, bool includeChildren = false) where T : Component
    {
        T result = includeChildren ? target.GetComponentInChildren<T>() : target.GetComponent<T>();

        if (result == null)
        {
            result = target.gameObject.AddComponent<T>();
        }

        return result;
    }
}
