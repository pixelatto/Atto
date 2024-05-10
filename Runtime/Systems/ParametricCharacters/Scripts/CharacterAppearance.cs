using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearance : MonoBehaviour
{

    public float timeToChangeVisualState = 0.01f;

    public float runAnimationSpeed = 2f;
    public float sprintAnimationSpeed = 2f;
    public float rollAnimationSpeed = 3f;

    Character character;
    Animator animator;

    public Character.CharacterStates visualState;

    void Awake()
    {
        character = GetComponentInParent<Character>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (character.currentStateTime > timeToChangeVisualState)
        {
            visualState = character.characterState;
        }
        if (animator.runtimeAnimatorController != null)
        {
            animator.Play(visualState.ToString().ToLower(), 0);
            animator.speed = 1f;
            switch (visualState)
            {
                case Character.CharacterStates.Idle:
                    break;
                case Character.CharacterStates.Walk:
                    animator.speed = 1f;
                    break;
                case Character.CharacterStates.Run:
                    animator.speed = runAnimationSpeed;
                    break;
                case Character.CharacterStates.Sprint:
                    animator.speed = sprintAnimationSpeed;
                    break;
                case Character.CharacterStates.Jump:
                    break;
                case Character.CharacterStates.Fall:
                    break;
                case Character.CharacterStates.Roll:
                    animator.speed = rollAnimationSpeed;
                    break;
            }
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
