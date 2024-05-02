using UnityEngine;

public class KeyboardController : Controller
{
    public KeyCode jumpKey = KeyCode.Z;

    override public bool jumpHeld => Input.GetKey(jumpKey);
    override public bool jumpPressed => Input.GetKeyDown(jumpKey);

    override public float horizontalAxis => Input.GetAxisRaw("Horizontal");
}