using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class GraySpriteShader : MonoBehaviour
{
    [Range(0, 1)]public float threshold = 0.5f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateMaterialProperties();
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer.material.HasProperty("_Threshold"))
        {
            spriteRenderer.material.SetFloat("_Threshold", threshold);
        }
    }
}