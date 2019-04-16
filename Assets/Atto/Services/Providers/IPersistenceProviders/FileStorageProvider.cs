using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class FileStorageProvider : IStorageService
{

    Encoding encoding = Encoding.UTF8;

    string dataPath = "";

    public FileStorageProvider(string path)
    {
        SetDestination(path);
    }

    public void SetDestination(string path)
    {
        dataPath = path;
    }

    public void WriteToStorage(string data)
    {
        BinaryWriter bw;

        Byte[] bytes = encoding.GetBytes(data);
        string content = Convert.ToBase64String(bytes);

        try
        {
            bw = new BinaryWriter(new FileStream(dataPath, FileMode.Create));
            bw.Write(content);
        }
        catch (IOException e)
        {
            Core.Logger.Log(e.Message);
            return;
        }
        bw.Close();
    }

    public string ReadFromStorage()
    {
        BinaryReader br;
        FileStream fs;
        string content;

        try
        {
            fs = new FileStream(dataPath, FileMode.OpenOrCreate);
            if (fs.Length > 0)
            {
                br = new BinaryReader(fs);
                byte[] bytes = Convert.FromBase64String(br.ReadString());
                content = encoding.GetString(bytes);
                return content;
            }
            else
            {
                return "";
            }
        }
        catch (Exception e)
        {
            Core.Logger.Log(e.Message);
            return "";
        }

    }

}
