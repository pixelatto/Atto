using System;
using System.Collections.Generic;
using UnityEngine;

public class VectorConverter<T> : FloatListConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }
}

public class Vector2MonolineConverter : VectorConverter<Vector2>
{
    protected override List<float> ObjectToList(object value)
    {
        var vector = (Vector2)value;
        return new List<float>() { vector.x, vector.y };
    }

    protected override object ListToObject(List<float> result)
    {
        return new Vector2(result[0], result[1]);
    }
}

public class Vector3MonolineConverter : VectorConverter<Vector3>
{
    protected override List<float> ObjectToList(object value)
    {
        var vector = (Vector3)value;
        return new List<float>() { vector.x, vector.y, vector.x };
    }

    protected override object ListToObject(List<float> result)
    {
        return new Vector3(result[0], result[1], result[2]);
    }
}

public class Vector4MonolineConverter : VectorConverter<Vector4>
{
    protected override List<float> ObjectToList(object value)
    {
        var vector = (Vector4)value;
        return new List<float>() { vector.x, vector.y, vector.z, vector.w };
    }

    protected override object ListToObject(List<float> result)
    {
        return new Vector4(result[0], result[1], result[2], result[3]);
    }
}