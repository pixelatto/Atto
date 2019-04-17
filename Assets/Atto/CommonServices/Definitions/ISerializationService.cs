using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializationService
{

    string Serialize<T>(T objectToSerialize);
    T Deserialize<T>(string data);

}