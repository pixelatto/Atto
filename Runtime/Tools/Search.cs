using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Utils
{
	public static class Search
	{
        public static List<T> NearbyComponents<T>(Vector3 position, float maxDistance) where T : Behaviour
        {
            List<T> result = new List<T>();

            var colliders = Physics2D.OverlapCircleAll(position, maxDistance);
            foreach (var collider in colliders)
            {
                var validTarget = collider.GetComponent<T>();
                if (validTarget != null)
                {
                    result.Add(validTarget);
                }
            }

            return result;
        }
    }
}

