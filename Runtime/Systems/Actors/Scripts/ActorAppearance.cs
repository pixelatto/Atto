using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAppearance : MonoBehaviour
{
    public ActorAnimations currentAnimation;

    public float timeToChangeVisualState = 0.01f;

    public float runAnimationSpeed = 2f;
    public float sprintAnimationSpeed = 2f;
    public float rollAnimationSpeed = 3f;

    public Actor character { get { if (character_ == null) { character_ = GetComponentInParent<Actor>(); }; return character_; } }
    private Actor character_;

    public Animator animator { get { if (animator_ == null) { animator_ = GetComponent<Animator>(); }; return animator_; } }
    private Animator animator_;

    void Update()
    {
        ClassifyAnimation();
        UpdateAnimation();
        UpdateFacing();
    }

    private void ClassifyAnimation()
    {
        if (character.currentState == ActorStates.Riding)
        {
            currentAnimation = ActorAnimations.Ride;
        }
        else if (character.isUnstable)
        {
            currentAnimation = ActorAnimations.Unstable;
        }
        else if (character.currentState == ActorStates.Rolling)
        {
            currentAnimation = ActorAnimations.Roll;
        }
        else if (character.isGrounded)
        {
            switch (character.horizontalMomentum)
            {
                case Momentum.None:
                    if (character.wantsToGoDown)
                    {
                        currentAnimation = ActorAnimations.Sit;
                    }
                    else
                    {
                        currentAnimation = ActorAnimations.Idle;
                    }
                    break;
                case Momentum.Slow:
                    if (character.wantsToGoDown && character.Can(Skill.Crawl))
                    {
                        currentAnimation = ActorAnimations.Crawl;
                    }
                    else
                    {
                        currentAnimation = ActorAnimations.Walk;
                    }
                    break;
                case Momentum.Medium:
                    currentAnimation = ActorAnimations.Run;
                    break;
                case Momentum.Fast:
                    currentAnimation = ActorAnimations.Sprint;
                    break;
            }
        }
        else // Airborne
        {
            if (character.isMovingUp)
            {
                currentAnimation = ActorAnimations.Jump;
            }
            else
            {
                currentAnimation = ActorAnimations.Fall;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator.runtimeAnimatorController != null)
        {
            animator.Play(currentAnimation.ToString().ToLower(), 0);
            animator.speed = 1f;
            switch (currentAnimation)
            {
                case ActorAnimations.Idle:
                    break;
                case ActorAnimations.Walk:
                    animator.speed = 1f;
                    break;
                case ActorAnimations.Run:
                    animator.speed = runAnimationSpeed;
                    break;
                case ActorAnimations.Sprint:
                    animator.speed = sprintAnimationSpeed;
                    break;
                case ActorAnimations.Jump:
                    break;
                case ActorAnimations.Fall:
                    break;
                case ActorAnimations.Roll:
                    animator.speed = rollAnimationSpeed;
                    break;
            }
        }
    }

    private void UpdateFacing()
    {
        if (character.facing == ActorFacing.Left)
        {
            LookLeft();
        }
        else if (character.facing == ActorFacing.Right)
        {
            LookRight();
        }
    }

    public void LookRight()
    {
        transform.localScale = new Vector3(+1, transform.localScale.y, transform.localScale.z);
    }

    public void LookLeft()
    {
        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }
}

public enum ActorAnimations { Idle, Walk, Run, Sprint, Jump, Fall, Sit, Crawl, Roll, Float, Levitate, LevitateJump, LevitateFall, Unstable, Look, Ride }