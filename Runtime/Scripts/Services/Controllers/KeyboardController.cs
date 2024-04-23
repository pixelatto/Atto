using UnityEngine;

public class KeyboardController : AController
{
    public KeyCode jumpKey = KeyCode.Z;

    override public bool wantsToJump => Input.GetKeyDown(jumpKey);
    override public float horizontalAxis => Input.GetAxis("Horizontal");
}