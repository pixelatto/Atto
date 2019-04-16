using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistenceService<T>
{
    void SetDestination(string path);
    T Read();
    void Write(T content);
    void Append(T content);
}
