using UnityEngine;

public abstract class Gadget : MonoBehaviour
{
    public GadgetState state = GadgetState.On;

    public Animator animator { get { if (animator_ == null) { animator_ = GetComponentInChildren<Animator>(); }; return animator_; } }
    private Animator animator_;

    GadgetState checkState;

    protected void Update()
    {
        if (state != checkState)
        {
            ChangeState(state);
        }

        switch (state)
        {
            case GadgetState.On:  On_State();  break;
            case GadgetState.Off: Off_State(); break;
            case GadgetState.Out: Out_State(); break;
        }
    }

    public void ChangeState(GadgetState state)
    {
        this.state = state;
        checkState = state;
        animator.Play(state.ToString().ToLowerInvariant());
    }

    public virtual void On_State()
    {

    }

    public virtual void Off_State()
    {

    }

    public virtual void Out_State()
    {

    }
}