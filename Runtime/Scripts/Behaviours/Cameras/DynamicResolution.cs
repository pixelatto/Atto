using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DynamicResolution : MonoBehaviour
{
    PixelCamera pixelCam;
    public Material ditheringMaterial;
    public RenderTexture renderTexture;

    void Update()
    {
        if (pixelCam == null)
        {
            pixelCam = GetComponent<PixelCamera>();
        }
        ditheringMaterial.SetFloat("_ScreenWidth", pixelCam.pixelWidth);
        ditheringMaterial.SetFloat("_ScreenHeight", pixelCam.pixelHeight);

        if (renderTexture.width != pixelCam.pixelWidth)
        {
            renderTexture.Release();
            renderTexture.width = pixelCam.pixelWidth;
            renderTexture.height = pixelCam.pixelHeight;
            renderTexture.Create();
        }
    }
}
