using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(Cleaner))]
public class CleanerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Cleaner cleaner = (Cleaner)target;

        if (!cleaner.enabled)
        {
            EditorGUILayout.LabelField(cleaner.hiddenComponentsInfo);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cleaner);
            cleaner.OnValidate();
        }
    }
}
#endif
