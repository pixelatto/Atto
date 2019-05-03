using System.IO;
using UnityEngine;

[BindService]
public class JsonSerializationProvider : ISerializationService
{
	public string Serialize<T>(T objectToSerialize)
	{
        return JsonUtility.ToJson(objectToSerialize);
    }

	public T Deserialize<T>(string data)
	{
        return JsonUtility.FromJson<T>(data);
    }
}