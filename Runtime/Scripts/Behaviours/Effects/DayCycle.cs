using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayCycle : MonoBehaviour
{
    public float dayTime = 0;
    public float dayDuration = 60;

    public float orbitalRadius = 100f;
    public float moonToSunOffset = 150f;

    public int anglesSubdivision = 360;

    [Header("References")]
    public PixelLight sun;
    public PixelLight moon;
    public Material daytimeTintMaterial;

    float normalizedDayPhase => (dayTime % dayDuration) / dayDuration;
    float dayAngle => anglesSubdivision != 0 ? 360f * Mathf.RoundToInt(normalizedDayPhase * anglesSubdivision) / ((float)(anglesSubdivision)) : normalizedDayPhase * 360f;

    float normalizedSunHeight;
    float normalizedMoonHeight;


    void Update()
    {
        //Sun
        if (sun != null)
        {
            sun.transform.localPosition = new Vector2().AngleToVector(dayAngle, orbitalRadius);
            sun.radiusInPixels = orbitalRadius * Global.pixelsPerUnit * 1.5f;
            normalizedSunHeight = Mathf.Clamp01(sun.transform.localPosition.y / orbitalRadius);
            sun.opacity = normalizedSunHeight;
            //sun.enabled = sun.transform.position.y > 0;
        }

        //Moon
        if (moon != null)
        {
            moon.transform.localPosition = new Vector2().AngleToVector(dayAngle - moonToSunOffset, orbitalRadius);
            moon.radiusInPixels = orbitalRadius * Global.pixelsPerUnit * 1.5f;
            normalizedMoonHeight = Mathf.Clamp01(moon.transform.localPosition.y / orbitalRadius);
            moon.opacity = normalizedMoonHeight;
            //moon.enabled = moon.transform.position.y > 0;
        }

        if (Application.isPlaying)
        {
            dayTime += Time.deltaTime;
        }

        daytimeTintMaterial.SetFloat("_Daytime", normalizedDayPhase);
    }

}
