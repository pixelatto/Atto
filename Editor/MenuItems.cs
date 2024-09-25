using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;

public class MenuItems
{
    public static void AddScenesToBuildSettings()
    {
        throw new NotImplementedException();
    }

    [MenuItem("Tools/Pixelatto/Clear player prefs")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}
#endif