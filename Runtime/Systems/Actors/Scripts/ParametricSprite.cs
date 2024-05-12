using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ParametricSprite : MonoBehaviour
{
    [Range(0, 10)] public int value = 0;
    public Color tint = Color.white;
    public bool lowThreeshold = false;

    public Animator syncToAnimator;

    int lastValue = 0;
    Color lastTint = Color.white;

    SpriteRenderer spriteRenderer { get { if (_spriteRenderer == null) { _spriteRenderer = GetComponent<SpriteRenderer>(); } return _spriteRenderer; } }
    SpriteRenderer _spriteRenderer;

    Animator animator { get { if (_animator == null) { _animator = GetComponent<Animator>(); } return _animator; } }
    Animator _animator;

    Actor character { get { if (_character == null) { _character = GetComponent<Actor>(); } return _character; } }
    Actor _character;

    MaterialPropertyBlock _propBlock;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (value != lastValue || tint != lastTint)
        {
            lastValue = value;
            lastTint = tint;
            Refresh();
        }

        if (syncToAnimator != null)
        {
            var stateInfo = syncToAnimator.GetCurrentAnimatorStateInfo(0);
            var playbackTime = stateInfo.normalizedTime;
            string animationName = syncToAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (HasAnimation(animator, animationName))
            {
                animator.Play(stateInfo.fullPathHash, 0, playbackTime);
            }
            else
            {
                animator.Play("idle", 0, 0f);
            }

            animator.Update(Time.deltaTime);
        }
    }

    bool HasAnimation(Animator animator, string animationName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
            {
                return true;
            }
        }
        return false;
    }

    public void SetParametes(int value, Color tint)
    {
        this.value = value;
        this.tint = tint;

        Refresh();
    }

    private void Refresh()
    {
        if (spriteRenderer.sharedMaterial == null || spriteRenderer.sharedMaterial.shader.name != "Atto/GrayscaleMask")
        {
            var tempMaterial = Resources.Load<Material>("Shaders/GrayscaleMask");
            spriteRenderer.sharedMaterial = tempMaterial;
        }
        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }
        spriteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_TopThreshold", (value) / 11f);
        _propBlock.SetFloat("_LowThreshold", lowThreeshold ? ((value - 0.5f) / 10f) : 0f);
        spriteRenderer.color = tint;
        spriteRenderer.SetPropertyBlock(_propBlock);
    }
}
