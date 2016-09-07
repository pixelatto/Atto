using System.Collections;
using System.Collections.Generic;
using System;
using Identifier = System.String;

public static partial class Services
{
	private const Identifier DEFAULT_SERVICE_ID = "base";

	private static Dictionary<Type, Dictionary<Identifier, Func<Service>>> serviceConstructors = null;
	private static Dictionary<Type, Dictionary<Identifier, Service>> serviceContainer = null;
	
	public static T Get<T>(Identifier id = DEFAULT_SERVICE_ID)
	{
		return (T)(Get(typeof(T), id));
	}

	public static void Provide<T>(Func<T> serviceConstructor) where T : Service
	{
		Provide(DEFAULT_SERVICE_ID, serviceConstructor);
	}

	public static void Provide<T>(Identifier id, Func<T> serviceConstructor) where T : Service
	{
		Provide(typeof(T), id, () =>
		{
			return serviceConstructor();
		});
	}

	private static object Get(Type type, Identifier id)
	{
		Service service = null;

		InitializeDictionaries();

		if (serviceConstructors.ContainsKey(type))
		{
			if (serviceConstructors[type].ContainsKey(id))
			{
				if (!serviceContainer.ContainsKey(type))
				{
					serviceContainer.Add(type, new Dictionary<Identifier, Service>());
				}

				if (serviceContainer[type].ContainsKey(id))
				{
					service = serviceContainer[type][id];
				}
				else
				{
					service = serviceConstructors[type][id]();

					serviceContainer[type].Add(id, service);
				}
			}
		}

		if (service == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Warning: App.Get: No service of type '{0}' with id '{1}' found", type.ToString(), id));
		}

		return service;
	}

	private static void Provide(Type type, Identifier id, Func<Service> serviceConstructor)
	{
		InitializeDictionaries();

		if (serviceConstructors.ContainsKey(type))
		{
			if (serviceConstructors[type].ContainsKey(id))
			{
				Services.Log.Warning("Warning, provided an existing service of type '{0}' with id '{1}'. The previous service has been unloaded.", type.ToString(), id);
			}
		}
		else
		{
			serviceConstructors.Add(type, new Dictionary<Identifier, Func<Service>>());
		}

		if (serviceConstructors[type].ContainsKey(id))
		{
			serviceConstructors[type][id] = serviceConstructor;

			if (serviceContainer.ContainsKey(type))
			{
				if (serviceContainer[type].ContainsKey(id))
				{
					serviceContainer[type].Remove(id);
				}
			}
		}
		else
		{
			serviceConstructors[type].Add(id, serviceConstructor);
		}
	}

	private static void InitializeDictionaries()
	{
		if (serviceConstructors == null)
		{
			serviceConstructors = new Dictionary<Type, Dictionary<Identifier, Func<Service>>>();
		}

		if (serviceContainer == null)
		{
			serviceContainer = new Dictionary<Type, Dictionary<Identifier, Service>>();
		}
	}
}
