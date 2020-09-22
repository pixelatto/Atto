using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace Atto.Serialization.Converters
{
	public class UriConverter : JsonConverter
	{
		const string coordinateSeparator = ", ";

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			string data = (string)value;
			data = data.Replace('\\', '/');
			data = data.Replace("//", "/");
			writer.WriteValue(data);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var tilesString = reader.Value.ToString();
			tilesString = tilesString.Replace('\\', '/');
			tilesString = tilesString.Replace("//", "/");
			return tilesString;
		}

		public override bool CanConvert(Type objectType) => objectType == typeof(string);
	}
}