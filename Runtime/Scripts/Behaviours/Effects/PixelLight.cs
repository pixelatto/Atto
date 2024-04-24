using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PixelLight : MonoBehaviour
{
    public Color color = Color.white;
    public float overrideRadius = 0f;
    public float overrideBrightness = 0f;
    public float radius => (ldtkFields != null) ? ldtkFields.GetFloat("Radius") / 8f : overrideRadius / 8f;
    public float brightness => (ldtkFields != null) ? ldtkFields.GetFloat("Brightness") : overrideBrightness;
    
    public float blinkFrequency = 0;
    public float blinkAmount = 0;

    [HideInInspector] public float phase = 0;

    DitherLightingPixelEffect ditherLightingPixelEffect;
    LDtkFields ldtkFields { get { if (_ldtkFields == null) { _ldtkFields = GetComponent<LDtkFields>(); } return _ldtkFields; } }
    LDtkFields _ldtkFields;


    private void Start()
    {
        phase = Random.value * 100;
        blinkFrequency += Random.Range(0.9f, 1.1f);
    }

    private void Update()
    {
        if (ditherLightingPixelEffect == null)
        {
            ditherLightingPixelEffect = FindObjectOfType<DitherLightingPixelEffect>();
        }

        if (ditherLightingPixelEffect != null)
        {
            if (ditherLightingPixelEffect.pixelCamera.worldRect.Contains(transform.position))
            {
                Draw.Circle(transform.position, 0.5f, Color.yellow);
                ditherLightingPixelEffect.SubscribeLight(this);
            }
            else
            {
                Draw.Circle(transform.position, 0.5f, Color.red*0.25f);
                ditherLightingPixelEffect.UnsubscribeLight(this);
            }
        }
    }

    private void OnDestroy()
    {
        if (ditherLightingPixelEffect != null)
        {
            ditherLightingPixelEffect.UnsubscribeLight(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
