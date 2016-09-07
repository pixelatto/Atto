using UnityEngine;
using System.Collections;

public abstract class SerializationService : Service {

	public abstract string Serialize<T>(T obj);
	public abstract T Deserialize<T>(string data);

}
