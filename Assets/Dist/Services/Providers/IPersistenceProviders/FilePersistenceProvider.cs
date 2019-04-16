using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FilePersistenceProvider : IPersistenceService<string>
{

    string dataPath = "";

    public void SetDestination(string path)
    {
        dataPath = path;
    }

    public string Read()
    {
        return File.ReadAllText(dataPath);
    }

    public void Write(string content)
    {
        File.WriteAllText(dataPath, content);
    }

    public void Append(string content)
    {
        File.AppendAllText(dataPath, content);
    }
}
