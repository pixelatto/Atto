using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DitherLightingPixelEffect : PixelEffect
{
    const int maxLightCount = 10;

    public List<PixelLight> lights = new List<PixelLight>();
    Vector4[] lightPositions = new Vector4[maxLightCount];
    float[] lightRadius = new float[maxLightCount];
    float[] lightBrightness = new float[maxLightCount];
    Vector4[] colors = new Vector4[maxLightCount];
    Material shadingMaterial;

    public void SubscribeLight(PixelLight pixelLight)
    {
        lights.AddIfNotListed(pixelLight);
    }

    public void UnsubscribeLight(PixelLight pixelLight)
    {
        lights.RemoveIfListed(pixelLight);
    }

    override protected void Update()
    {
        base.Update();
        UpdateShader();
    }

    private void UpdateShader()
    {
        UpdateSprite();
        for (int i = lights.Count - 1; i >= 0 ; i--)
        {
            var currentLight = lights[i];
            if (currentLight != null)
            {
                lightPositions[i] = new Vector4(currentLight.transform.position.x, currentLight.transform.position.y, 0, 0);
                lightRadius[i] = currentLight.radiusInUnits;
                float blink = 0;
                if (currentLight.blinkAmount != 0)
                {
                    currentLight.phase += Time.deltaTime * currentLight.blinkFrequency;
                    blink = (1f + Mathf.Sin(currentLight.phase)) * 0.5f * currentLight.blinkAmount;
                }
                lightBrightness[i] = currentLight.brightness + blink;
                colors[i] = currentLight.color;

                if (shadingMaterial == null)
                {
                    shadingMaterial = Resources.Load<Material>("Shaders/DitherLighting");
                }
            }
            else
            {
                lights.RemoveAt(i);
            }
        }

        for (int i = lights.Count; i < maxLightCount; i++)
        {
            lightPositions[i] = Vector4.zero;
            lightBrightness[i] = 0;
        }

        shadingMaterial.SetVectorArray("_LightPoints", lightPositions);
        shadingMaterial.SetVectorArray("_Colors", colors);
        shadingMaterial.SetFloatArray("_LightRadius", lightRadius);
        shadingMaterial.SetFloatArray("_LightBrightness", lightBrightness);
    }
}
