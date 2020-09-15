using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

public class JsonSerializationService : ISerializationService
{
    public string formatExtension => ".json";

    public T Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }

    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}
