using UnityEngine;

[SelectionBase, ExecuteAlways]
public class PixelPositioner : MonoBehaviour
{
    public Transform visualsBase;
    public bool horizontally = true;
    public bool vertically = true;

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (visualsBase != null)
            {
                visualsBase.rotation = Quaternion.identity;
                visualsBase.localPosition = Vector3.zero;
                visualsBase.position =
                    new Vector3(
                        horizontally ? Mathf.Round(visualsBase.position.x * 8f) / 8f : visualsBase.position.x,
                        vertically ? Mathf.Round(visualsBase.position.y * 8f) / 8f : visualsBase.position.y,
                        visualsBase.position.z
                        );
            }
        }
        else
        {
            if (visualsBase != null)
            {
                transform.position =
                    new Vector3(
                        Mathf.Round(visualsBase.position.x * 4f) / 4f,
                        Mathf.Round(visualsBase.position.y * 4f) / 4f,
                        transform.position.z
                        );
            }
            else
            {
                transform.position = new Vector3(
                    Mathf.Round(transform.position.x * 8f) / 8f,
                    Mathf.Round(transform.position.y * 8f) / 8f,
                    transform.position.z
                    );
            }
        }
    }
}