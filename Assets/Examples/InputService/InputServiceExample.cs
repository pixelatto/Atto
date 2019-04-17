using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputServiceExample : MonoBehaviour , IControllable
{

    public IDevice device;

    void Start ()
    {
        device = GetComponent<UnityKeyboardDevice>();
        Core.Input.AssignControllable(device, this);
    }

    public void Control(IDevice controller)
    {
        var hor = controller.State.Horizontal.Value;
        var ver = controller.State.Vertical.Value;
        transform.position += hor * Vector3.right * Time.deltaTime * 10f;
        transform.position += ver * Vector3.up * Time.deltaTime * 10f;
    }

}
