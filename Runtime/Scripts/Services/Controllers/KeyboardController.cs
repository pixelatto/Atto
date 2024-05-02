using UnityEngine;

public class KeyboardController : Controller
{
    public KeyCode jumpKey = KeyCode.Z;
    public KeyCode actionKey = KeyCode.X;

    override public bool jumpHeld => Input.GetKey(jumpKey);
    override public bool jumpPressed => Input.GetKeyDown(jumpKey);

    override public bool actionHeld => Input.GetKey(actionKey);
    override public bool actionPressed => Input.GetKeyDown(actionKey);

    override public float horizontalAxis => Input.GetAxisRaw("Horizontal");
}