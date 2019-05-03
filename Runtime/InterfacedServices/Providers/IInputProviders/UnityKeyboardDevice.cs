using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityKeyboardDevice : MonoBehaviour, IDevice
{
    public Guid Id { get { return guid; } }
    Guid guid = new Guid();

    public IPlayer Owner { get; set; }
    public string DeviceName { get; set; }
    public List<IControllable> Slaves { get; set; }

    public DeviceState State { get; private set; }
    DeviceState previousState;
	
	void Update ()
    {
        UpdateState();
        ControlSlaves();
    }

    public void UpdateState()
    {
        previousState = State;
        State = new DeviceState();

        UpdateAxis(AxisType.Vertical, TwoKeyCodedAxis(KeyCode.UpArrow, KeyCode.DownArrow));
        UpdateAxis(AxisType.Horizontal, TwoKeyCodedAxis(KeyCode.RightArrow, KeyCode.LeftArrow));
        UpdateAxis(AxisType.Jump, OneKeyCodedAxis(KeyCode.Z));
        UpdateAxis(AxisType.Action, OneKeyCodedAxis(KeyCode.X));
        UpdateAxis(AxisType.Submit, OneKeyCodedAxis(KeyCode.Return));
        UpdateAxis(AxisType.Cancel, OneKeyCodedAxis(KeyCode.Escape));
    }

    private void UpdateAxis(AxisType axisType, float newValue)
    {
        AxisState previousAxisState = new AxisState(0, InputState.Idle);
        if (previousState != null && previousState.axisList.ContainsKey(axisType))
        {
            previousAxisState = previousState.axisList[axisType];
        }
        AxisState newAxisState = new AxisState(newValue, InputState.Idle);
        newAxisState.SetInputState(GetNewInputState(previousAxisState, newAxisState));
        State.axisList.Add(axisType, newAxisState);
    }

    public void ControlSlaves()
    {
        foreach (var slave in Slaves)
        {
            slave.Control(this);
        }
    }

    float OneKeyCodedAxis(KeyCode positive)
    {
        float result = 0;
        if (Input.GetKey(positive))
        {
            result = 1f;
        }
        else
        {
            result = 1f;
        }
        return result;
    }

    float TwoKeyCodedAxis(KeyCode positive, KeyCode negative)
    {
        float result = 0;
        if (Input.GetKey(positive))
        {
            result += 1f;
        }
        if (Input.GetKey(negative))
        {
            result -= 1f;
        }
        return result;
    }

    InputState GetNewInputState(AxisState previousState, AxisState currentState)
    {
        InputState result = InputState.Undefined;
        if (previousState.Value == 0 && currentState.Value != 0)
        {
            result = InputState.Pressed;
        }
        else if (previousState.Value != 0 && currentState.Value == 0)
        {
            result = InputState.Released;
        }
        else if (previousState.Value != 0 && currentState.Value != 0)
        {
            result = InputState.Held;
        }
        else
        {
            result = InputState.Idle;
        }
        return result;
    }
}
