using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteAlways]
public class Fader : MonoBehaviour
{
    public bool isCovering = false;

    public float duration = 0.35f;

    SpriteRenderer spriteRenderer { get { if (spriteRenderer_ == null) { spriteRenderer_ = GetComponent<SpriteRenderer>(); }; return spriteRenderer_; } }
    SpriteRenderer spriteRenderer_;

    Tweener tweener;

    private void Update()
    {

    }

    [Button]
    public void FadeToggle()
    {
        if (isCovering)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    [Button]
    public void FadeIn()
    {
        if (isCovering)
        {
            if (Application.isPlaying)
            {
                if (tweener != null)
                {
                    tweener.Kill();
                }
                tweener = spriteRenderer.sharedMaterial.DOFloat(0f, "_Threshold", duration);
            }
            else
            {
                spriteRenderer.sharedMaterial.SetFloat("_Threshold", 0f);
            }
            isCovering = false;
        }
    }

    [Button]
    public void FadeOut()
    {
        if (!isCovering)
        {
            if (Application.isPlaying)
            {
                if (tweener != null)
                {
                    tweener.Kill();
                }
                tweener = spriteRenderer.sharedMaterial.DOFloat(1f, "_Threshold", duration);
            }
            else
            {
                spriteRenderer.sharedMaterial.SetFloat("_Threshold", 1f);
            }
            isCovering = true;
        }
    }
}
