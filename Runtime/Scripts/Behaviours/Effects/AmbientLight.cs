using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AmbientLight : MonoBehaviour
{

    [Range(0, 8)] public float targetBrightness = 8;
    public float tweenSpeed = 1f;

    float currentBrightness = 8;
    float levels = 8;

    SpriteRenderer sr;

    private void Start()
    {
        currentBrightness = targetBrightness;
    }

    void Update()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        var error = targetBrightness - currentBrightness;

        if (Application.isPlaying && tweenSpeed != 0)
        {
            if (Mathf.Abs(error) > 0.01f)
            {
                currentBrightness += Mathf.Sign(error) * Time.deltaTime * tweenSpeed;
            }
        }
        else
        {
            currentBrightness += error;
        }

        var value = currentBrightness / levels;
        sr.color = new Color(value, value, value);
    }
}
