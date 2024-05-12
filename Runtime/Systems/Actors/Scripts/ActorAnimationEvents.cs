using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimationEvents : MonoBehaviour
{

    public Actor character { get { if (character_ == null) { character_ = GetComponentInParent<Actor>(); }; return character_; } }
    private Actor character_;

    void OnRunFootstep()
    {
        character.OnRunFootstep();
    }
}
