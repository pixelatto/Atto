using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Hjson;

namespace Atto.Services
{
	public class HjsonSerializationService : ISerializationService
	{
		public string formatExtension => ".hjson";

		public T Deserialize<T>(string hjsonData)
		{
			var jsonValue = HjsonValue.Parse(hjsonData);
			var jsonString = jsonValue.ToString(Stringify.Formatted);
			return JsonConvert.DeserializeObject<T>(jsonString);
		}

		public string Serialize<T>(T obj)
		{
			var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
			var hjsonString = HjsonValue.Parse(jsonString).Qo();
			return hjsonString.ToString(Stringify.Hjson);
		}

	}
}
