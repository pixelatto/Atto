using UnityEngine;

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
				Debug.LogWarning(string.Format("[Singleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T)));

				return null;
			}

			lock (instanceLock)
			{
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
                        Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");

						return instance;
					}

					if (instance == null)
					{
						var singleton = new GameObject();
						instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) " + typeof(T).ToString();

						DontDestroyOnLoad(singleton);

						Debug.Log(string.Format("[Singleton] An instance of {0} is needed in the scene, so '{1}' was created with DontDestroyOnLoad.", typeof(T), singleton));
					}
					else
					{
                        //Debug.Log("[Singleton] Using instance already created: {0}", instance.gameObject.name);
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
