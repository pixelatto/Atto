using System.Xml.Serialization;
using System.IO;

public class XmlSerializationProvider : ISerializationService
{
	public string Serialize<T>(T objectToSerialize)
	{
		var stringWriter = new StringWriter();
		var serializer = new XmlSerializer(typeof(T));

		serializer.Serialize(stringWriter, objectToSerialize);
		stringWriter.Close();

		return stringWriter.ToString();
	}

	public T Deserialize<T>(string data)
	{
		var stringReader = new StringReader(data);
		var serializer = new XmlSerializer(typeof(T));

		T result = (T)serializer.Deserialize(stringReader);
		stringReader.Close();

		return result;
	}
}