using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;

[CustomPropertyDrawer(typeof(Observable<>), true)]
public class ObservableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        EditorGUI.PropertyField(position, valueProperty, label, true);
        EditorGUI.EndProperty();
    }
}
#endif