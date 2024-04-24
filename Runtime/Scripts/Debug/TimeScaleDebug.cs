using UnityEngine;

public class TimeScaleDebug : MonoBehaviour
{
    public KeyCode slowDownKey = KeyCode.PageUp;
    public KeyCode speedUpKey = KeyCode.PageDown;


    void Update()
    {
        if (Application.isEditor)
        {
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