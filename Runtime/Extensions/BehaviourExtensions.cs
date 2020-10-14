using UnityEngine;

namespace Atto.Extensions
{
	public static class BehaviourExtensions
	{
		public static T GetOrAddComponent<T>(this Component target) where T : Component
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