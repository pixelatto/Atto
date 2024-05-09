using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverrides : MonoBehaviour
{

    public float playbackRate = 1;
    public bool randomizeStartTime = false;

    public Animator animator { get { if (animator_ == null) { animator_ = GetComponent<Animator>(); }; return animator_; } }
    private Animator animator_;

    void Start()
    {
        if (randomizeStartTime)
        {
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float randomStartTime = Random.Range(0.0f, 1.0f);
                animator.Play(stateInfo.fullPathHash, 0, randomStartTime);
            }
        }
    }

    void Update()
    {
        animator.speed = playbackRate;
    }
}
