using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraPostprocessing : MonoBehaviour
{
    public Material EffectMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (EffectMaterial)
            Graphics.Blit(src, dest, EffectMaterial);
        else
            Graphics.Blit(src, dest);
    }

}
