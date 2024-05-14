using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformInspectorExtension : Editor
{
    public override void OnInspectorGUI()
    {
        // Llamamos al inspector original
        DrawDefaultInspector();

        // Añadimos un botón personalizado
        if (GUILayout.Button("Botón Personalizado"))
        {
            Transform transform = (Transform)target;
            // Aquí puedes añadir la funcionalidad que desees
            Debug.Log("Botón Personalizado Presionado en " + transform.name);
        }
    }
}
