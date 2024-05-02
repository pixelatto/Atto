using UnityEngine;

public class KeyboardController : AController
{
    public KeyCode jumpKey = KeyCode.Z;

    override public bool jumpHeld => Input.GetKey(jumpKey);
    override public bool jumpPressed => Input.GetKeyDown(jumpKey);

    override public bool horizontalHeld => horizontalAxis != 0;

    override public float horizontalAxis => Input.GetAxisRaw("Horizontal");
}