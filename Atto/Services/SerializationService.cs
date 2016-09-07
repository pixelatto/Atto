using UnityEngine;
using System.Collections;

namespace Atto.Services {

	public abstract class SerializationService : Service {

		public abstract string Serialize<T>(T obj);
		public abstract T Deserialize<T>(string data);

	}

}
