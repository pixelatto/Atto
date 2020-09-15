using Ju;
using Ju.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializationService : IService
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string data);

    string formatExtension { get; }
}
