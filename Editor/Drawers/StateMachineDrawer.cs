using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(StateMachine<>), true)]
public class StateMachineDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var currentStateLabelProp = property.FindPropertyRelative("currentStateLabel");
        EditorGUI.PropertyField(position, currentStateLabelProp, label);
    }
}
#endif