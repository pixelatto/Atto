using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearance : MonoBehaviour
{
    float timeToChangeVisualState = 0.01f;

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
        animator.Play(visualState.ToString().ToLower(), 0);
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
