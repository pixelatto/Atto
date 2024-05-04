using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CellularRasterizer : MonoBehaviour
{
    public LayerMask rasterMask;
    public List<CameraPostprocessing> postprocessings;
    public CellularAutomata cellularAutomata;

    public static CellularRasterizer instance;

    private void Awake()
    {
        instance = this;
    }

    public static Texture2D RasterTerrain(Camera camera)
    {
        var savedMask = camera.cullingMask;
        camera.cullingMask = instance.rasterMask;
        foreach (var postProcessing in instance.postprocessings)
        {
            postProcessing.enabled = false;
        }

        var savedBgColor = camera.backgroundColor;
        var savedClearFlags = camera.clearFlags;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;

        RenderTexture rt = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0);
        camera.targetTexture = rt;
        camera.Render();
        Texture2D screenCapture = new Texture2D(camera.pixelWidth, camera.pixelHeight, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenCapture.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
        screenCapture.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        camera.backgroundColor = savedBgColor;
        camera.clearFlags = savedClearFlags;
        
        /*
        //Save raster to file
        byte[] bytes = screenCapture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "Raster.png");
        File.WriteAllBytes(path, bytes);
        */

        foreach (var postProcessing in instance.postprocessings)
        {
            postProcessing.enabled = true;
        }
        camera.cullingMask = savedMask;

        return screenCapture;
    }

}
