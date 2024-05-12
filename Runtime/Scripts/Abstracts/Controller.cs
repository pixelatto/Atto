using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    
    public IControllable target { get { if (target_ == null) { target_ = GetComponent<IControllable>(); }; return target_; } }
    private IControllable target_;

    public abstract bool jumpPressed { get; }
    public abstract bool jumpHeld { get; }
    public abstract bool actionHeld { get; }
    public abstract bool actionPressed { get; }

    public abstract float horizontalAxis { get; }
    public abstract float verticalAxis { get; }

    private void Start()
    {

    }

    public void Update()
    {
        target.SetControl(this);
    }

    public void OverrideControl(IControllable newTarget)
    {
        target_ = newTarget;
    }

    public void RestoreControl()
    {
        target_ = GetComponent<IControllable>();
    }
}
