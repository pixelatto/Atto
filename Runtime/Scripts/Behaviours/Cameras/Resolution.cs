using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Resolution : MonoBehaviour
{
    public Material ditheringMaterial;
    public RenderTexture renderTexture;

    PixelCamera pixelCam;

    void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                Screen.SetResolution(128, 72, false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                Screen.SetResolution(128 * 2, 72 * 2, false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Screen.SetResolution(128 * 4, 72 * 4, false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                Screen.SetResolution(128 * 8, 72 * 8, false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                Screen.SetResolution(128 * 10, 72 * 10, false);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Screen.SetResolution(128 * 12, 72 * 12, false);
            }
        }

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
