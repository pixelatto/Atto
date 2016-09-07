using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Atto.Services
{
	public class XmlSerializationService : SerializationService
	{
		override public string Serialize<T>(T objectToSerialize)
		{
			StringWriter stringWriter=new StringWriter();
			var serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(stringWriter, objectToSerialize);
			stringWriter.Close();
			return stringWriter.ToString();
		}

		override public T Deserialize<T>(string data)
		{
			StringReader stringReader=new StringReader(data);
			var serializer = new XmlSerializer(typeof(T));
			T result=(T)serializer.Deserialize(stringReader);
			stringReader.Close();
			return result;
		}
	}
}
