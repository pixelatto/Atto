
#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Get the target object
        MonoBehaviour target = (MonoBehaviour)serializedObject.targetObject;
        System.Type type = target.GetType();

        // Find all methods marked with ButtonAttribute
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            if (method.GetCustomAttribute(typeof(ButtonAttribute)) != null)
            {
                // Draw button with the method name
                if (GUILayout.Button(method.Name))
                {
                    // Invoke the method
                    method.Invoke(target, null);
                }
            }
        }
    }
}
#endif

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{

}
