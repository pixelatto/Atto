using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericListMonolineConverter<T> : JsonConverter
{
    protected virtual string separatorString => ", ";

    protected virtual List<T> ObjectToList(object value)
    {
        return (List<T>)value;
    }

    protected virtual object ListToObject(List<T> result)
    {
        return result;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var stringValue = reader.Value.ToString();
        if (stringValue == null) return null;
        string[] stringSeparators = new string[] { separatorString };
        string[] stringValues = stringValue.Split(stringSeparators, StringSplitOptions.None);
        List<T> result = new List<T>();
        foreach (var value in stringValues)
        {
            result.Add(Parse(value));
        }

        return ListToObject(result);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        List<T> items = ObjectToList(value);
        var dataString = "";
        foreach (var item in items)
        {
            dataString += item.ToString();
            dataString += separatorString;
        }
        dataString = dataString.TrimEnd(separatorString.ToCharArray());
        writer.WriteValue(dataString);
    }

    protected abstract T Parse(string stringValue);

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<T>);
    }
}