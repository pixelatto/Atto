using UnityEngine;

namespace Atto
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		private static object instanceLock = new object();
		private static bool applicationIsQuitting = false;

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					Core.Logger.Warning("[Singleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T));

					return null;
				}

				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Core.Logger.Error("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");

							return instance;
						}

						if (instance == null)
						{
							var singleton = new GameObject();
							instance = singleton.AddComponent<T>();
							singleton.name = "(singleton) " + typeof(T).ToString();

							DontDestroyOnLoad(singleton);

							Core.Logger.Log("[Singleton] An instance of {0} is needed in the scene, so '{1}' was created with DontDestroyOnLoad.", typeof(T), singleton);
						}
						else
						{
							//Core.Log.Debug("[Singleton] Using instance already created: {0}", _instance.gameObject.name);
						}
					}

					return instance;
				}
			}
		}

		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}
}
