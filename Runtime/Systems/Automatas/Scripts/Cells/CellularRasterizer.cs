using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CellularRasterizer : MonoBehaviour
{
    bool saveToFile = false;
    
    public Camera rasterizerCamera { get { if (rasterizerCamera_ == null) { rasterizerCamera_ = GetComponent<Camera>(); }; return rasterizerCamera_; } }
    private Camera rasterizerCamera_;

    public static CellularRasterizer instance { get { if (instance_ == null) { instance_ = CreateRasterizer(); } return instance_; } }
    static CellularRasterizer instance_;

    public static CellularRasterizer CreateRasterizer()
    {
        var cellularRasterizer = new GameObject("CellularRasterizer");
        cellularRasterizer.transform.SetParent(CellularAutomata.instance.transform);

        var cameraComponent = cellularRasterizer.AddComponent<Camera>();
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        cameraComponent.backgroundColor = Color.black;
        cameraComponent.cullingMask = Global.terrainMask;
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = 4.5f;
        cameraComponent.nearClipPlane = 0.3f;
        cameraComponent.farClipPlane = 1000f;
        cameraComponent.depth = -2;

        var result = cellularRasterizer.AddComponent<CellularRasterizer>();

        return result;
    }

    public Texture2D RasterChunk(CellularChunk chunk)
    {
        rasterizerCamera.enabled = true;
        rasterizerCamera.transform.position = chunk.transform.position + (Vector3)chunk.worldSize * 0.5f + Vector3.back * 10;
        rasterizerCamera.clearFlags = CameraClearFlags.SolidColor;
        rasterizerCamera.backgroundColor = Color.clear;

        RenderTexture rt = new RenderTexture(chunk.pixelSize.x, chunk.pixelSize.y, 0);
        rasterizerCamera.targetTexture = rt;
        rasterizerCamera.Render();
        Texture2D screenCapture = new Texture2D(chunk.pixelSize.x, chunk.pixelSize.y, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenCapture.ReadPixels(new Rect(0, 0, chunk.pixelSize.x, chunk.pixelSize.y), 0, 0);
        screenCapture.Apply();
        rasterizerCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        
        //Save raster to file
        if (saveToFile)
        {
            byte[] bytes = screenCapture.EncodeToPNG();
            string path = Path.Combine(Application.dataPath, "Raster_" + chunk.name + ".png");
            File.WriteAllBytes(path, bytes);
        }

        rasterizerCamera.enabled = false;

        return screenCapture;
    }

}
