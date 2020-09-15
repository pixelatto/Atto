using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class BoxInt3D
{
    [JsonConverter(typeof(Vector3IntMonolineConverter))]
    public Vector3Int position { get; private set; }
    [JsonConverter(typeof(Vector3IntMonolineConverter))]
    public Vector3Int size { get; private set; }

    [JsonIgnore]
    public int x => position.x;
    [JsonIgnore]
    public int y => position.y;
    [JsonIgnore]
    public int z => position.z;

    [JsonIgnore]
    public int width => size.x;
    [JsonIgnore]
    public int height => size.y;
    [JsonIgnore]
    public int depth => size.z;

    [JsonIgnore]
    public int xMin => x;
    [JsonIgnore]
    public int yMin => y;
    [JsonIgnore]
    public int zMin => z;

    [JsonIgnore]
    public int xMax => x + width;
    [JsonIgnore]
    public int yMax => y + height;
    [JsonIgnore]
    public int zMax => z + depth;

    [JsonIgnore]
    public Vector3 center => new Vector3(x + width * 0.5f, y + height * 0.5f, z + depth * 0.5f);

    public BoxInt3D(int x, int y, int z, int width, int height, int depth) : this(new Vector3Int(x, y, z), new Vector3Int(width, height, depth)) { }
    public BoxInt3D(Vector3Int position, Vector3Int size)
    {
        this.position = position;
        this.size = size;
    }

    public bool Contains(Vector3Int position)
    {
        return position.x >= xMin && position.y >= yMin && position.z >= zMin && position.x < xMax && position.y < yMax && position.z < zMax;
    }

    public Vector3Int Clamp(Vector3Int position, bool excludeUpperBound = false)
    {
        var delta = 0;
        if (excludeUpperBound)
        {
            delta = -1;
        }

        var x = Mathf.Clamp(position.x, xMin, xMax + delta);
        var y = Mathf.Clamp(position.y, yMin, yMax + delta);
        var z = Mathf.Clamp(position.z, zMin, zMax + delta);
        return new Vector3Int(x, y, z);
    }

    public override string ToString()
    {
        return $"Box3D({x}, {y}, {z}, {width}, {height}, {depth})";
    }
}