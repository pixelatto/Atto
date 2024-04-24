using UnityEngine;

public class PixelCamera : MonoBehaviour
{
    public float zoom = 1;
    public float baseCameraSize = 4.5f;
    public int basePixelHeight = 72;
    public int basePixelWidth = 128;

    float aspectRatio => (float)basePixelWidth / (float)basePixelHeight;
    public int pixelWidth => (int)(basePixelWidth * zoom);
    public int pixelHeight => (int)(basePixelHeight * zoom);

    public int pixelsPerWorldUnit = 8;
    public float worldPixelSize => 1f / (float)pixelsPerWorldUnit;

    public Camera unityCamera { get { if (_unityCamera == null) { _unityCamera = GetComponent<Camera>(); }; return _unityCamera; } }
    Camera _unityCamera;


    public Rect worldRect
    {
        get
        {
            var size = new Vector2(baseCameraSize * 2f * aspectRatio, baseCameraSize * 2f) * zoom;
            var position = new Vector2(transform.position.x, transform.position.y) - new Vector2(size.x * 0.5f, size.y * 0.5f);
            return new Rect(position, size);
        }
    }
}