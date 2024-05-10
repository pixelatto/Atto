using UnityEngine;

public abstract class Gadget : MonoBehaviour
{
    public GadgetState state = GadgetState.On;

    public Animator animator { get { if (animator_ == null) { animator_ = GetComponentInChildren<Animator>(); }; return animator_; } }
    private Animator animator_;

    public void ChangeState(GadgetState state)
    {
        this.state = state;
        animator.Play(state.ToString().ToLowerInvariant());
    }

}