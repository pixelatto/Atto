using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputService
{

    void ConnectDevice(IDevice device);
    void DisconnectDevice(IDevice device);

    IDevice GetDeviceState(Guid deviceId);
    List<IDevice> GetConnectedDevices();

    void AssignPlayer(IPlayer player, IDevice device);

    void AssignControllable(IDevice device, IControllable controllable);
    void UnassignControllable(IDevice device, IControllable controllable);

}

public interface IDevice
{
    Guid Id { get; }
    IPlayer Owner { get; set; }
    string DeviceName { get; set; }
    List<IControllable> Slaves { get; set; }
    DeviceState State { get; }
    void UpdateState();
    void ControlSlaves();
}

public interface IPlayer
{
    int Id { get; set; }
    string PlayerName { get; set; }
}

public interface IControllable
{
    void Control(IDevice controller);
}

public class DeviceState
{
    public Dictionary<AxisType, AxisState> axisList = new Dictionary<AxisType, AxisState>();

    public AxisState Vertical   { get { return axisList[AxisType.Vertical]; } }
    public AxisState Horizontal { get { return axisList[AxisType.Horizontal]; } }

    public AxisState Jump { get { return axisList[AxisType.Jump]; } }
    public AxisState Fire { get { return axisList[AxisType.Fire]; } }
    public AxisState Action { get { return axisList[AxisType.Action]; } }
    public AxisState Special { get { return axisList[AxisType.Special]; } }

    public AxisState LeftBumper { get { return axisList[AxisType.LeftBumper]; } }
    public AxisState RightBumper { get { return axisList[AxisType.RightBumper]; } }
    public AxisState LeftTrigger { get { return axisList[AxisType.LeftTrigger]; } }
    public AxisState RightTrigger { get { return axisList[AxisType.RightTrigger]; } }

    public AxisState VerticalSecondary { get { return axisList[AxisType.VerticalSecondary]; } }
    public AxisState HorizontalSecondary { get { return axisList[AxisType.HorizontalSecondary]; } }

    public AxisState Submit { get { return axisList[AxisType.Submit]; } }
    public AxisState Cancel { get { return axisList[AxisType.Cancel]; } }
}

public struct AxisState
{
    public float Value { get; private set; }
    public InputState State { get; private set; }

    public AxisState(float value, InputState state)
    {
        this.Value = value;
        this.State = state;
    }

    public void SetInputState(InputState state)
    {
        this.State = state;
    }

    public bool IsIdle { get { return State == InputState.Idle; } }
    public bool IsPressed { get { return State == InputState.Pressed; } }
    public bool IsHeld { get { return State == InputState.Held; } }
    public bool IsReleased { get { return State == InputState.Released; } }
}

public enum AxisType { Undefined, Vertical, Horizontal, Jump, Fire, Action, Special, LeftBumper, RightBumper, LeftTrigger, RightTrigger, VerticalSecondary, HorizontalSecondary, Submit, Cancel }
public enum InputState { Undefined, Idle, Pressed, Held, Released }