using UnityEngine;

public class PixelCamera : MonoBehaviour
{
    public float zoom = 1;
    public float baseCameraSize = 4.5f;
    public int basePixelHeight = 72;
    public int basePixelWidth = 128;
    public int viewPortLookaheadPixels = 32;

    float aspectRatio => (float)basePixelWidth / (float)basePixelHeight;
    public int pixelWidth => (int)(basePixelWidth * zoom);
    public int pixelHeight => (int)(basePixelHeight * zoom);

    public int pixelsPerWorldUnit = 8;
    public float worldPixelSize => 1f / (float)pixelsPerWorldUnit;

    public Camera unityCamera { get { if (_unityCamera == null) { _unityCamera = GetComponent<Camera>(); }; return _unityCamera; } }
    Camera _unityCamera;

    public Vector3 cameraBottomLeft { get; private set; }
    public Vector2Int pixelBottomLeft { get; private set; }

    public RectInt pixelRect { get; private set; }
    public Rect worldRect { get; private set; }

    public RectInt lookAheadPixelRect { get; private set; }
    public Rect lookAheadWorldRect { get; private set; }

    private void Update()
    {
        cameraBottomLeft = unityCamera.transform.position + new Vector3(-unityCamera.orthographicSize * 16f / 9f, -unityCamera.orthographicSize, 0);
        pixelBottomLeft = new Vector2Int(Mathf.RoundToInt(cameraBottomLeft.x * 8f), Mathf.RoundToInt(cameraBottomLeft.y * 8f));
        pixelRect = new RectInt(pixelBottomLeft.x, pixelBottomLeft.y, CellularAutomata.roomPixelSize.x, CellularAutomata.roomPixelSize.y);
        worldRect = new Rect(cameraBottomLeft.x, cameraBottomLeft.y, CellularAutomata.roomPixelSize.x / 8f, CellularAutomata.roomPixelSize.y / 8f);
        lookAheadPixelRect = pixelRect.Grow(viewPortLookaheadPixels);
        lookAheadWorldRect = worldRect.Grow(viewPortLookaheadPixels / 8f);

        var size = new Vector2(baseCameraSize * 2f * aspectRatio, baseCameraSize * 2f) * zoom;
        var position = new Vector2(transform.position.x, transform.position.y) - new Vector2(size.x * 0.5f, size.y * 0.5f);
        worldRect = new Rect(position, size);

        Draw.Rect(lookAheadWorldRect, Color.cyan);
    }
}