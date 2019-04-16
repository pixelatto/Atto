using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using RSG;

public class BinaryDatabaseProvider : IDataBaseService
{
    private static readonly string IdFormat = "db_{0}";
    Encoding encoding = Encoding.UTF8;

    const string encodingKey = "db_encoding";
    const string versionKey = "db_gameversion";

    private Dictionary<string, string> data;

    IStorageService databaseStorage;
    ISerializationService serialization;

    public BinaryDatabaseProvider(string path, IStorageService storage, ISerializationService serializationProvider)
    {
        databaseStorage = storage;
        serialization = serializationProvider;
        LoadFromStorage();
    }

    void LoadFromStorage()
    {
        string plainData = databaseStorage.ReadFromStorage();
        data = serialization.Deserialize<Dictionary<string, string>>(plainData);
    }

    void SaveToStorage()
    {
        string serializedData = serialization.Serialize(data);
        databaseStorage.WriteToStorage(serializedData);
    }

    public IPromise<T> ReadEntry<T>(string id, T defaultValue = default(T))
    {
        Promise<T> result = new Promise<T>();
        string dbId = GetDbId(id);

        if (data.ContainsKey(dbId))
        {
            try
            {
                object value = Core.Serialization.Deserialize<T>(data[dbId]);
                result.Resolve((T)value);
            }
            catch
            {
                result.Reject(new InvalidCastException(string.Format("Entry with id '{0}' is not a {1} value", id, typeof(T).ToString())));
            }
        }
        else
        {
            if (defaultValue != null)
            {
                result.Resolve(defaultValue);
            }
            else
            {
                result.Reject(new ArgumentException(string.Format("Entry with id '{0}' not found in database.", id)));
            }
        }

        return result;
    }

    public void WriteEntry<T>(string id, T value)
    {
        string dbId = GetDbId(id);
        data[dbId] = Core.Serialization.Serialize(value);

        SetDataKey(encodingKey, encoding.ToString());
        SetDataKey(versionKey, Application.version);

        SaveToStorage();
    }

    public IPromise<bool> HasEntry(string id)
    {
        Promise<bool> result = new Promise<bool>();
        string dbId = GetDbId(id);

        result.Resolve(data.ContainsKey(dbId));

        return result;
    }

    private string GetDbId(string id)
    {
        return string.Format(IdFormat, id);
    }

    void SetDataKey(string key, string value)
    {
        if (!data.ContainsKey(key))
        {
            data.Add(key, value);
        }
        else
        {
            data[key] = value;
        }
    }

    private bool IsBasicType(Type type)
    {
        return type.Equals(typeof(int)) || type.Equals(typeof(float)) || type.Equals(typeof(string));
    }
}
