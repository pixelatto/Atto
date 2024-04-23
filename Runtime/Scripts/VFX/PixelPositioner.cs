using UnityEngine;

[SelectionBase, ExecuteAlways]
public class PixelPositioner : MonoBehaviour
{
    public Transform visualsBase;

    private void Update()
    {
        if (Application.isPlaying)
        {
            visualsBase.rotation = Quaternion.identity;
            visualsBase.localPosition = Vector3.zero;
            visualsBase.position =
                new Vector3(
                    Mathf.Round(visualsBase.position.x * 8f) / 8f,
                    Mathf.Round(visualsBase.position.y * 8f) / 8f,
                    Mathf.Round(visualsBase.position.z * 8f) / 8f
                    );
        }
        else
        {
            transform.position =
                new Vector3(
                    Mathf.Round(visualsBase.position.x * 4f) / 4f,
                    Mathf.Round(visualsBase.position.y * 4f) / 4f,
                    Mathf.Round(visualsBase.position.z * 4f) / 4f
                    );
        }
    }
}