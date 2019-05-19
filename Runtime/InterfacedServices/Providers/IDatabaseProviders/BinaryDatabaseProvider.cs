using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using RSG;

[BindService]
public class BinaryDatabaseProvider : IDatabaseService
{
    private static readonly string IdFormat = "db_{0}";
    Encoding encoding = Encoding.UTF8;

    const string encodingKey = "db_encoding";
    const string appVersionKey = "db_appversion";
    const string dbVersionKey = "db_version";

    private Dictionary<string, string> data;

    IStorageService storageProvider;
    ISerializationService serializationProvider;
    IDataChannelService dataChannelProvider;

    DataChannel currentDataChannel;

    public BinaryDatabaseProvider()
    {
        this.storageProvider = Atto.Get<IStorageService>();
        this.serializationProvider = Atto.Get<ISerializationService>();
        this.dataChannelProvider = Atto.Get<IDataChannelService>();
        currentDataChannel = dataChannelProvider.GetChannel(DataChannelTypes.Database);

        LoadFromStorage();
    }

    void LoadFromStorage()
    {
        string plainData = storageProvider.ReadFromStorage(currentDataChannel);
        data = serializationProvider.Deserialize<Dictionary<string, string>>(plainData);
    }

    void SaveToStorage()
    {
        UpdateMetadata();
        string serializedData = serializationProvider.Serialize(data);
        storageProvider.WriteToStorage(serializedData, currentDataChannel);
    }

    private void UpdateMetadata()
    {
        string currentVersionString = "0";
        data.TryGetValue(dbVersionKey, out currentVersionString);
        int currentVersionNumber = 0;
        int.TryParse(currentVersionString, out currentVersionNumber);
        currentVersionNumber++;
        SetDataKey(dbVersionKey, currentVersionNumber.ToString());
        SetDataKey(encodingKey, encoding.ToString());
        SetDataKey(appVersionKey, Application.version);
    }

    public IPromise<T> ReadEntry<T>(string id, T defaultValue = default(T))
    {
        Promise<T> result = new Promise<T>();
        string dbId = GetDbId(id);

        if (data.ContainsKey(dbId))
        {
            try
            {
                object value = serializationProvider.Deserialize<T>(data[dbId]);
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

    public void DeleteEntry<T>(string entryKey)
    {
        if (data.ContainsKey(entryKey))
        {
            data.Remove(entryKey);
        }
    }

    public void WriteEntry<T>(string id, T value)
    {
        string dbId = GetDbId(id);
        data[dbId] = serializationProvider.Serialize(value);

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
