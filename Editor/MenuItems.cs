
using UnityEditor;
using UnityEngine;

public class MenuItems
{
    [MenuItem("Tools/Pixelatto/Clear player prefs")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}