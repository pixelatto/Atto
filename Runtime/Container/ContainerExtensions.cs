using System;
using UnityEngine;
using Identifier = System.String;

public static class ContainerExtensions
{
    private const Identifier DEFAULT_ID = "base";

    public static T Get<T>(this Container container)
	{
		return container.Get<T>(DEFAULT_ID);
	}

	public static void Provide<T>(this Container container, Func<T> classConstructor)
	{
        var previousProvider = container.Get<T>(DEFAULT_ID);
        if (previousProvider != null)
        {
            Debug.LogError("Can't provide service of type: " + typeof(T).Name + " because it's already provided by " + previousProvider.GetType().Name);
            return;
        }

        container.Provide<T>(DEFAULT_ID, classConstructor);
	}

	public static void ProvideFactory<T>(this Container container, Func<T> classConstructor)
	{
		container.ProvideFactory<T>(DEFAULT_ID, classConstructor);
	}
}