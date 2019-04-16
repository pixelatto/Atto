using Atto.Services;
using System;
using Identifier = System.String;

public static partial class Core
{
	public static Container Container { get { return container; } }
	private static Container container = new Container();

	public static T Get<T>()
	{
		return container.Get<T>();
	}

	public static T Get<T>(Identifier id)
	{
		return container.Get<T>(id);
	}

	public static void Provide<T>(Func<T> classConstructor)
	{
		container.Provide<T>(classConstructor);
	}

	public static void Provide<T>(Identifier id, Func<T> classConstructor)
	{
		container.Provide<T>(id, classConstructor);
	}

	public static void ProvideFactory<T>(Func<T> classConstructor)
	{
		container.ProvideFactory<T>(classConstructor);
	}

	public static void ProvideFactory<T>(Identifier id, Func<T> classConstructor)
	{
		container.ProvideFactory<T>(id, classConstructor);
	}

	public static ILogService Logger
	{
		get { return container.Get<ILogService>(); }
	}

    public static ISerializationService Serialization
    {
        get { return container.Get<ISerializationService>(); }
    }
}
