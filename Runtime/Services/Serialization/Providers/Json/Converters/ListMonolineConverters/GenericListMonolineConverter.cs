using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Serialization.Converters
{
	public abstract class GenericListMonolineConverter<T> : JsonConverter
	{
		protected virtual string separatorString => ", ";
		protected virtual string leftBracket => "";
		protected virtual string rightBracket => "";

		protected virtual List<T> ObjectToList(object value) => (List<T>)value;
		protected virtual object ListToObject(List<T> result) => result;
		public override bool CanConvert(Type objectType) => objectType == typeof(List<T>);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			try
			{
				return ConvertRead(TryExtractBrackets(reader.Value.ToString()));
			}
			catch (FormatException)
			{
				throw new FormatException($"Used wrong brackets for {reader.Path}, Converter uses \"{leftBracket}\" and \"{rightBracket}\"");
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> writer.WriteValue(ConvertToWrite(value));

		public virtual object ConvertRead(string stringValue)
		{
			string[] stringSeparators = new string[] { separatorString };
			string[] stringValues = stringValue.Split(stringSeparators, StringSplitOptions.None);
			List<T> result = new List<T>();
			foreach (var value in stringValues)
			{
				result.Add(Parse(value));
			}

			return ListToObject(result);
		}

		public virtual string ConvertToWrite(object value)
		{
			List<T> items = ObjectToList(value);
			var dataString = "";
			if (leftBracket.Length > 0 && rightBracket.Length > 0)
			{
				dataString += leftBracket;
			}

			foreach (var item in items)
			{
				dataString += item.ToString();
				dataString += separatorString;
			}

			dataString = dataString.TrimEnd(separatorString.ToCharArray());

			if (leftBracket.Length > 0 && rightBracket.Length > 0)
			{
				dataString += rightBracket;
			}

			return dataString;
		}

		protected abstract T Parse(string stringValue);

		private string TryExtractBrackets(string value)
		{
			if (leftBracket.Length > 0 && rightBracket.Length > 0)
			{
				string valueLeftDelimitator = value.Substring(0, leftBracket.Length);
				string valueRightDelimitator = value.Substring(value.Length - rightBracket.Length, rightBracket.Length);

				if (valueLeftDelimitator == leftBracket && valueRightDelimitator == rightBracket)
				{
					value = value.Substring(leftBracket.Length, value.Length - (leftBracket.Length + rightBracket.Length));
				}
				else
				{
					throw new FormatException();
				}
			}

			return value;
		}
	}
}