using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class BinaryDatabaseProvider : IDataBaseService
{
    private static readonly string IdFormat = "db_{0}";

    private Dictionary<string, string> data;
    private string savePath;

    const string utf8Tag = "UTF8";
    const string asciiTag = "ASCII";

    const string encodingKey = "db_encoding";
    const string versionKey = "db_gameversion";

    public BinaryDatabaseProvider(string path)
    {
        savePath = path;
        LoadData();
    }

    public T Load<T>(string id, T defaultValue = default(T)) where T : new()
    {
        T result = new T();
        string dbId = GetDbId(id);

        if (data.ContainsKey(dbId))
        {
            try
            {
                T value = Core.Serialization.Deserialize<T>(data[dbId]);
                result = value;
            }
            catch
            {
                throw new UnityException("Couldn't deserialize database");
            }
        }
        else
        {
            if (defaultValue != null)
            {
                return defaultValue;
            }
            else
            {
                throw new UnityException(string.Format("Entry with id '{0}' not found in database.", id));
            }
        }

        return result;
    }

    public void Save<T>(string id, T value)
    {
        string dbId = GetDbId(id);
        data[dbId] = Core.Serialization.Serialize(value);
        SynchronizeFile();
    }

    public bool HasEntry(string id)
    {
        string dbId = GetDbId(id);
        return data.ContainsKey(dbId);
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

    private void SynchronizeFile()
    {
        BinaryWriter bw;

        SetDataKey(encodingKey, utf8Tag);
        SetDataKey(versionKey, Application.version);

        Byte[] bytes = Encoding.UTF8.GetBytes(Core.Serialization.Serialize(data));
        string content = Convert.ToBase64String(bytes);
        
        try
        {
            bw = new BinaryWriter(new FileStream(savePath, FileMode.Create));
            bw.Write(content);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
            return;
        }
        bw.Close();
    }

    private void LoadData()
    {
        BinaryReader br;
        FileStream fs;
        string content;

        try
        {
            fs = new FileStream(savePath, FileMode.OpenOrCreate);
            if (fs.Length > 0)
            {
                br = new BinaryReader(fs);
                byte[] bytes = Convert.FromBase64String(br.ReadString());
                content = Encoding.ASCII.GetString(bytes);
                data = Core.Serialization.Deserialize<Dictionary<string, string>>(content);

                //If we don't find any encoding, we asume ASCII
                if (!data.ContainsKey(encodingKey))
                {
                    data.Add(encodingKey, asciiTag);
                }
                else
                {
                    if (data[encodingKey] == utf8Tag)
                    {
                        content = Encoding.UTF8.GetString(bytes);
                        data = Core.Serialization.Deserialize<Dictionary<string, string>>(content);
                    }
                }
            }
            else
            {
                data = new Dictionary<string, string>();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            data = new Dictionary<string, string>();
            return;
        }

    }

    private bool IsBasicType(Type type)
    {
        return type.Equals(typeof(int)) || type.Equals(typeof(float)) || type.Equals(typeof(string));
    }
}
