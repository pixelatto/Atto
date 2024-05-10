using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{

    public Character character { get { if (character_ == null) { character_ = GetComponentInParent<Character>(); }; return character_; } }
    private Character character_;

    void OnRunFootstep()
    {
        character.OnRunFootstep();
    }
}
