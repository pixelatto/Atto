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
                        horizontally ? Mathf.Round(visualsBase.position.x * Global.pixelsPerUnit) / Global.pixelsPerUnit : visualsBase.position.x,
                        vertically ? Mathf.Round(visualsBase.position.y * Global.pixelsPerUnit) / Global.pixelsPerUnit : visualsBase.position.y,
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
                        Mathf.Round(visualsBase.position.x * Global.pixelsPerUnit / 2) / (Global.pixelsPerUnit / 2),
                        Mathf.Round(visualsBase.position.y * Global.pixelsPerUnit / 2) / (Global.pixelsPerUnit / 2),
                        transform.position.z
                        );
            }
            else
            {
                transform.position = new Vector3(
                    Mathf.Round(transform.position.x * Global.pixelsPerUnit) / Global.pixelsPerUnit,
                    Mathf.Round(transform.position.y * Global.pixelsPerUnit) / Global.pixelsPerUnit,
                    transform.position.z
                    );
            }
        }
    }
}