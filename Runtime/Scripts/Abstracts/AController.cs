using UnityEngine;

public abstract class AController : MonoBehaviour
{
    public GameObject overrideControllable;
    IControllable target;

    public abstract bool wantsToWalk { get; }
    public abstract bool wantsToJump { get; }
    public abstract float horizontalAxis { get; }

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
