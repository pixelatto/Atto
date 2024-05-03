using UnityEngine;

public class KeyboardController : Controller
{
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;

    public KeyCode jumpKey = KeyCode.Z;
    public KeyCode actionKey = KeyCode.X;

    override public bool jumpHeld => Input.GetKey(jumpKey);
    override public bool jumpPressed => Input.GetKeyDown(jumpKey);

    override public bool actionHeld => Input.GetKey(actionKey);
    override public bool actionPressed => Input.GetKeyDown(actionKey);

    override public float horizontalAxis => (Input.GetKey(leftKey) ? -1f : 0f) + (Input.GetKey(rightKey) ? 1f : 0f);
    override public float verticalAxis => (Input.GetKey(downKey) ? -1f : 0f) + (Input.GetKey(upKey) ? 1f : 0f);
}