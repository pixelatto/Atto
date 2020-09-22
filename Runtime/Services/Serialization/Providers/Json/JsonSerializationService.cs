using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Atto.Services
{
	public class JsonSerializationService : ISerializationService
	{
		public string formatExtension => ".json";

		public T Deserialize<T>(string data) => JsonConvert.DeserializeObject<T>(data);

		public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
	}
}
