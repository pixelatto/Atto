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
    float[] lightLuminosity = new float[maxLightCount];
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
                lightRadius[i] = currentLight.radius;
                float blink = 0;
                if (currentLight.blinkAmount > 0)
                {
                    currentLight.phase += Time.deltaTime * currentLight.blinkFrequency;
                    blink = Mathf.Sin(currentLight.phase) * currentLight.blinkAmount;
                }
                lightLuminosity[i] = currentLight.brightness + blink;

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
            lightLuminosity[i] = 0;
        }

        shadingMaterial.SetVectorArray("_LightPoints", lightPositions);
        shadingMaterial.SetFloatArray("_LightRadius", lightRadius);
        shadingMaterial.SetFloatArray("_LightLuminosity", lightLuminosity);
    }
}
