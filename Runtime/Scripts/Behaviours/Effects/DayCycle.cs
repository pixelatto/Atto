using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayCycle : MonoBehaviour
{
    public float dayTime = 0;
    public float dayDuration = 60;

    public float orbitalRadius = 100f;
    public float sunBrightness = 8;
    public float moonBrightness = 4;
    [Range(0, 5)]public float dayAmbientLight = 4;
    public int anglesSubdivision = 360;
    public float maxIntensityBoost = 1.5f;

    [Header("References")]
    public PixelLight sun;
    public PixelLight moon;
    public Material colorRampMaterial;
    public Material lightmasksMaterial;

    float normalizedDayPhase => (dayTime % dayDuration) / dayDuration;
    float dayAngle => anglesSubdivision != 0 ? 360f * Mathf.RoundToInt(normalizedDayPhase * anglesSubdivision) / ((float)(anglesSubdivision)) : normalizedDayPhase * 360f;
    AmbientLight ambientLight;

    void Update()
    {
        if (ambientLight == null)
        {
            ambientLight = FindObjectOfType<AmbientLight>();
        }

        float normalizedSunHeight = 0;
        float normalizedMoonHeight = 0;

        //Sun
        if (sun != null)
        {
            sun.transform.localPosition = new Vector2().AngleToVector(dayAngle, orbitalRadius);
            sun.overrideRadius = (orbitalRadius + Camera.main.orthographicSize * 3f) * 8f;
            normalizedSunHeight = Mathf.Clamp01(sun.transform.localPosition.y / orbitalRadius);
            float adjustedSunHeight = Mathf.Clamp01(normalizedSunHeight - 0.05f);
            sun.overrideBrightness = sunBrightness * adjustedSunHeight;
            sun.arc = 15f;
            //sun.enabled = sun.transform.position.y > 0;
        }

        //Moon
        if (moon != null)
        {
            moon.transform.localPosition = new Vector2().AngleToVector(dayAngle + 180, orbitalRadius);
            normalizedMoonHeight = Mathf.Clamp01(moon.transform.localPosition.y / orbitalRadius);
            float adjustedMoonHeight = Mathf.Clamp01(normalizedMoonHeight - 0.05f);
            moon.overrideBrightness = moonBrightness * adjustedMoonHeight;
            moon.arc = 15f;
            //moon.enabled = moon.transform.position.y > 0;
        }

        if (Application.isPlaying)
        {
            dayTime += Time.deltaTime;
        }

        //Ambient light
        float ambientLightFactor = Mathf.Clamp01((normalizedSunHeight - 0.15f) * 10);
        ambientLight.targetBrightness = Mathf.Lerp(0, dayAmbientLight, ambientLightFactor);

        colorRampMaterial.SetFloat("_Daytime", normalizedDayPhase);
        lightmasksMaterial.SetFloat("_Intensify", Mathf.Lerp(1f, maxIntensityBoost, Mathf.Sqrt(normalizedSunHeight))); ;
    }

}
