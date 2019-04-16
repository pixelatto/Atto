using UnityEngine;

namespace Atto
{
	public static class MonoBehaviourExtensions
	{
		public static T GetOrAddComponent<T>(this MonoBehaviour target) where T : Component
		{
			T result = target.GetComponent<T>();

			if (result == null)
			{
				result = target.gameObject.AddComponent<T>();
			}

			return result;
		}
	}
}
