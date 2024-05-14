using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LabelAttribute labelAttribute = (LabelAttribute)attribute;
        label.text = labelAttribute.label;
        EditorGUI.PropertyField(position, property, label, true);
    }
}
