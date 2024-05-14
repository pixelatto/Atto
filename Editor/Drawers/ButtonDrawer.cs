using UnityEngine;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MonoBehaviour target = (MonoBehaviour)serializedObject.targetObject;
        System.Type type = target.GetType();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            if (method.GetCustomAttribute(typeof(ButtonAttribute)) != null)
            {
                if (GUILayout.Button(method.Name))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}
