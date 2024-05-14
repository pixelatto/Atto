using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class SimpleFullModeToggler
{
    private static bool simpleMode = false;
    private static Dictionary<GameObject, HideFlags> originalHideFlags = new Dictionary<GameObject, HideFlags>();

    // Lista de tipos a ocultar
    private static readonly System.Type[] typesToHide = {
        typeof(Rigidbody2D),
        typeof(SpriteRenderer),
        typeof(Collider2D),
        typeof(Camera),
        typeof(AudioListener),
        typeof(ParticleSystem),
        typeof(Transform)
    };

    static SimpleFullModeToggler()
    {
        Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnFinishedDefaultHeaderGUI(Editor editor)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(simpleMode ? "Show all components" : "Show scripts only", GUILayout.Width(150)))
        {
            ToggleMode(editor.targets);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeGameObject != null)
        {
            ApplyMode(Selection.objects, simpleMode);
        }
    }

    private static void ToggleMode(Object[] targets)
    {
        simpleMode = !simpleMode;
        ApplyMode(targets, simpleMode);
    }

    private static void ApplyMode(Object[] targets, bool simpleMode)
    {
        foreach (var target in targets)
        {
            GameObject gameObject = target as GameObject;
            if (gameObject != null)
            {
                if (simpleMode)
                {
                    HideComponents(gameObject);
                }
                else
                {
                    RestoreOriginalComponents(gameObject);
                }
                EditorUtility.SetDirty(gameObject);
            }
        }

        // Refresh the editor window to apply the changes
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.RepaintProjectWindow();
        foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
        {
            window.Repaint();
        }
    }

    private static void HideComponents(GameObject gameObject)
    {
        SaveOriginalHideFlags(gameObject);

        foreach (var type in typesToHide)
        {
            var components = gameObject.GetComponents(type);
            foreach (var component in components)
            {
                component.hideFlags = HideFlags.HideInInspector;
            }
        }
    }

    private static void RestoreOriginalComponents(GameObject gameObject)
    {
        if (originalHideFlags.ContainsKey(gameObject))
        {
            gameObject.hideFlags = originalHideFlags[gameObject];
        }

        foreach (var type in typesToHide)
        {
            var components = gameObject.GetComponents(type);
            foreach (var component in components)
            {
                component.hideFlags = HideFlags.None;
            }
        }
    }

    private static void SaveOriginalHideFlags(GameObject gameObject)
    {
        if (!originalHideFlags.ContainsKey(gameObject))
        {
            originalHideFlags[gameObject] = gameObject.hideFlags;
        }
    }
}
