using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class RectIntMonolineConverter : IntListConverter
{
    protected override List<int> ObjectToList(object value)
    {
        var rect = (RectInt)value;
        return new List<int>() { rect.x, rect.y, rect.width, rect.height };
    }

    protected override object ListToObject(List<int> result)
    {
        return new RectInt(result[0], result[1], result[2], result[3]);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RectInt);
    }
}