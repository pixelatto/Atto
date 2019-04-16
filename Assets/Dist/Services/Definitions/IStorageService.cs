using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorageService
{
    void SetDestination(string path);
    string ReadFromStorage();
    void WriteToStorage(string content);
}
