using UnityEngine;

public class TimeScaleDebug : MonoBehaviour
{

    public KeyCode resetKey = KeyCode.Home;
    public KeyCode slowDownKey = KeyCode.PageUp;
    public KeyCode speedUpKey = KeyCode.PageDown;

    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(resetKey))
            {
                Time.timeScale = 1f;
            }
            if (Input.GetKeyDown(speedUpKey))
            {
                Time.timeScale *= 2f;
            }
            else if (Input.GetKeyDown(slowDownKey))
            {
                Time.timeScale /= 2f;
            }
        }
    }
}