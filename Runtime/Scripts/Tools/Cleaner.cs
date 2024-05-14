using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Cleaner : MonoBehaviour
{
    public string hiddenComponentsInfo;

    private void Start()
    {
        
    }

    public void OnValidate()
    {
        UpdateHiddenComponentsInfo();
        ToggleComponents(enabled);
    }

    private void UpdateHiddenComponentsInfo()
    {
        hiddenComponentsInfo = "Hidden Components: ";
        int hiddenCount = 0;

        if (GetComponent<Rigidbody2D>() && !enabled)
        {
            hiddenComponentsInfo += "Rigidbody2D ";
            hiddenCount++;
        }
        if (GetComponent<SpriteRenderer>() && !enabled)
        {
            hiddenComponentsInfo += "SpriteRenderer ";
            hiddenCount++;
        }
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            if (!enabled)
            {
                hiddenComponentsInfo += collider.GetType().Name + " ";
                hiddenCount++;
            }
        }

        if (hiddenCount == 0)
        {
            hiddenComponentsInfo += "None";
        }
    }

    private void ToggleComponents(bool show)
    {
        transform.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        if (GetComponent<Rigidbody2D>())
        {
            GetComponent<Rigidbody2D>().hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        }
        if (GetComponent<SpriteRenderer>())
        {
            GetComponent<SpriteRenderer>().hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        }
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        }
        EditorUtility.SetDirty(this);
    }

    private void OnDrawGizmos()
    {
        if (!enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawIcon(transform.position, "d_console.warnicon", true);
        }
    }
}
