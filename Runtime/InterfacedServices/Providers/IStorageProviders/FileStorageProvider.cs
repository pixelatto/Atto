using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

[BindService]
public class FileStorageProvider : IStorageService
{

    ILogService logger;

    Encoding encoding = Encoding.UTF8;

    DataChannel defaultStorageChannel = new DataChannel() { uri = Application.dataPath + "/defaultStorage.sav" };

    public FileStorageProvider()
    {
        logger = Atto.Get<ILogService>();
    }

    public void WriteToStorage(string data, DataChannel channel = null)
    {
        if (channel == null)
        {
            channel = defaultStorageChannel;
        }

        BinaryWriter bw;

        Byte[] bytes = encoding.GetBytes(data);
        string content = Convert.ToBase64String(bytes);

        try
        {
            bw = new BinaryWriter(new FileStream(channel.uri, FileMode.Create));
            bw.Write(content);
        }
        catch (IOException e)
        {
            logger.Log(e.Message);
            return;
        }
        bw.Close();
    }

    public string ReadFromStorage(DataChannel channel = null)
    {
        if (channel == null)
        {
            channel = defaultStorageChannel;
        }

        BinaryReader br;
        FileStream fs;
        string content;

        try
        {
            fs = new FileStream(channel.uri, FileMode.OpenOrCreate);
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
            logger.Log(e.Message);
            return "";
        }

    }
    
}
