using System;
using System.Collections.Generic;
using Identifier = System.String;

namespace Atto
{
	public delegate void CoreClassReplaced(Type type, Identifier id);

	public static partial class Core
	{
		public static event CoreClassReplaced OnClassReplaced;

		private static Dictionary<Type, Dictionary<Identifier, Func<object>>> classConstructors = null;
		private static Dictionary<Type, Dictionary<Identifier, Func<object>>> classFactories = null;
		private static Dictionary<Type, Dictionary<Identifier, object>> instanceContainer = null;

		public static T Get<T>(Identifier id)
		{
			return (T)(Get(typeof(T), id));
		}

		public static void Provide<T>(Identifier id, Func<T> classConstructor)
		{
			Provide(typeof(T), id, () =>
			{
				return classConstructor();
			});
		}

		public static void ProvideFactory<T>(Identifier id, Func<T> classConstructor)
		{
			ProvideFactory(typeof(T), id, () =>
			{
				return classConstructor();
			});
		}

		private static object Get(Type type, Identifier id)
		{
			InitializeDictionaries();

			object instance = null;

			if(classFactories.ContainsKey(type) && classFactories[type].ContainsKey(id))
			{
				instance = classFactories[type][id]();
			}
			else
			{
				if(classConstructors.ContainsKey(type) && classConstructors[type].ContainsKey(id))
				{
					if(instanceContainer.ContainsKey(type) && instanceContainer[type].ContainsKey(id))
					{
						instance = instanceContainer[type][id];
					}
					else
					{
						instance = classConstructors[type][id]();

						instanceContainer[type].Add(id, instance);
					}
				}
			}

			if(instance == null)
			{
				throw new NullReferenceException(string.Format("Warning: Core.Get: No class of type '{0}' with id '{1}' found", type.ToString(), id));
			}

			return instance;
		}

		private static void Provide(Type type, Identifier id, Func<object> classConstructor)
		{
			InitializeDictionaries();

			if(CheckDuplicatedClass(type, id))
			{
				OnClassReplaced(type, id);
			}

			SetClassConstructor(type, id, classConstructor);
		}

		private static void ProvideFactory(Type type, Identifier id, Func<object> classConstructor)
		{
			InitializeDictionaries();

			if(CheckDuplicatedClass(type, id))
			{
				OnClassReplaced(type, id);
			}

			SetClassFactory(type, id, classConstructor);
		}

		private static void InitializeDictionaries()
		{
			if(classConstructors == null)
			{
				classConstructors = new Dictionary<Type, Dictionary<Identifier, Func<object>>>();
			}

			if(classFactories == null)
			{
				classFactories = new Dictionary<Type, Dictionary<Identifier, Func<object>>>();
			}

			if(instanceContainer == null)
			{
				instanceContainer = new Dictionary<Type, Dictionary<Identifier, object>>();
			}
		}

		private static bool CheckDuplicatedClass(Type type, Identifier id)
		{
			bool duplicated = false;

			if(classConstructors.ContainsKey(type))
			{
				duplicated |= classConstructors[type].ContainsKey(id);
			}

			if(classFactories.ContainsKey(type))
			{
				duplicated |= classFactories[type].ContainsKey(id);
			}

			return duplicated;
		}

		private static void SetClassConstructor(Type type, Identifier id, Func<object> classConstructor)
		{
			RemoveClassFactory(type, id);

			if(classConstructors.ContainsKey(type))
			{
				RemoveClassConstructor(type, id);
			}
			else
			{
				classConstructors.Add(type, new Dictionary<Identifier, Func<object>>());
			}
			
			classConstructors[type].Add(id, classConstructor);
		}

		private static void RemoveClassConstructor(Type type, Identifier id)
		{
			if(classConstructors.ContainsKey(type))
			{
				classConstructors[type].Remove(id);

				if(instanceContainer.ContainsKey(type))
				{
					instanceContainer[type].Remove(id);
				}
			}
		}

		private static void SetClassFactory(Type type, Identifier id, Func<object> classConstructor)
		{
			RemoveClassConstructor(type, id);

			if(classFactories.ContainsKey(type))
			{
				RemoveClassFactory(type, id);
			}
			else
			{
				classFactories.Add(type, new Dictionary<Identifier, Func<object>>());
			}

			classFactories[type].Add(id, classConstructor);
		}

		private static void RemoveClassFactory(Type type, Identifier id)
		{
			if(classFactories.ContainsKey(type))
			{
				classFactories[type].Remove(id);
			}
		}
	}
}
