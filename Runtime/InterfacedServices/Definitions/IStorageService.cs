using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorageService
{
    string ReadFromStorage(DataChannel channel);
    void WriteToStorage(string content, DataChannel channel);
}
