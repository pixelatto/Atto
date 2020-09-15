using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHexConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color color = (Color)value;
        var hexString = (color == Color.clear) ? string.Empty : ("#" + ColorUtility.ToHtmlStringRGBA(color));
        writer.WriteValue(hexString);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var hexString = reader.Value.ToString();
        if (hexString == null || !hexString.StartsWith("#")) return Color.clear;
        Color color;
        ColorUtility.TryParseHtmlString(hexString, out color);
        return color;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color);
    }
}