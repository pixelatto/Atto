using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
