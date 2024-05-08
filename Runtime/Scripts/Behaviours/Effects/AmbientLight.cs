using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AmbientLight : MonoBehaviour
{
    [Range(0, 1)] public float baseAmbientLight = 0.2f;

    [HideInInspector] public float attenuation = 1;

    SpriteRenderer spriteRenderer { get { if (spriteRenderer_ == null) { spriteRenderer_ = GetComponent<SpriteRenderer>(); }; return spriteRenderer_; } }
    SpriteRenderer spriteRenderer_;


    void Start()
    {
        
    }

    void Update()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, baseAmbientLight * attenuation);
    }
}
