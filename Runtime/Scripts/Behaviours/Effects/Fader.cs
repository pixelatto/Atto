using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteAlways]
public class Fader : MonoBehaviour
{
    public bool isCovering = false;
    public float duration = 0.35f;
    public AmbientLight ambientLight;

    MeshRenderer meshRenderer { get { if (meshRenderer_ == null) { meshRenderer_ = GetComponent<MeshRenderer>(); }; return meshRenderer_; } }
    MeshRenderer meshRenderer_;

    Tweener tweener;

    public static Fader instance;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        var fadeAmount = 1f - meshRenderer.sharedMaterial.GetFloat("_Threshold");
        ambientLight.attenuation = fadeAmount;
    }

    [Button]
    public void FadeToggle()
    {
        if (isCovering)
        {
            FadeIn(null);
        }
        else
        {
            FadeOut(null);
        }
    }

    [Button]
    public void FadeIn()
    {
        FadeIn(null);
    }

    [Button]
    public void FadeOut()
    {
        FadeOut(null);
    }

    public void FadeIn(System.Action OnComplete)
    {
        if (isCovering)
        {
            if (Application.isPlaying)
            {
                if (tweener != null)
                {
                    tweener.Kill();
                }
                tweener = meshRenderer.sharedMaterial.DOFloat(0f, "_Threshold", duration).OnComplete(() => { if (OnComplete != null) { OnComplete(); } });
            }
            else
            {
                meshRenderer.sharedMaterial.SetFloat("_Threshold", 0f);
            }
            isCovering = false;
        }
    }

    public void FadeOut(System.Action OnComplete)
    {
        if (!isCovering)
        {
            if (Application.isPlaying)
            {
                if (tweener != null)
                {
                    tweener.Kill();
                }
                tweener = meshRenderer.sharedMaterial.DOFloat(1f, "_Threshold", duration).OnComplete(() => { if (OnComplete != null) { OnComplete(); } });
            }
            else
            {
                meshRenderer.sharedMaterial.SetFloat("_Threshold", 1f);
            }
            isCovering = true;
        }
    }
}
