using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public GameObject overrideControllable;
    IControllable target;

    public abstract bool jumpPressed { get; }
    public abstract bool jumpHeld { get; }
    public abstract bool actionHeld { get; }
    public abstract bool actionPressed { get; }

    public abstract float horizontalAxis { get; }
    public abstract float verticalAxis { get; }

    private void Start()
    {
        if (overrideControllable == null)
        {
            overrideControllable = gameObject;
        }
        target = overrideControllable.GetComponent<IControllable>();
    }

    public void Update()
    {
        target.Control(this);
    }

}
