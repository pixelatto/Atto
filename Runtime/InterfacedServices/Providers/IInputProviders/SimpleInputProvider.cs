using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[BindService]
public class SimpleInputProvider : IInputService
{
    ILogService logger;

    List<IDevice> devices = new List<IDevice>();

    public SimpleInputProvider()
    {
        logger = Atto.Get<ILogService>();
    }

    public void AssignControllable(IDevice device, IControllable controllable)
    {
        if (device.Slaves == null)
        {
            device.Slaves = new List<IControllable>();
        }
        device.Slaves.Add(controllable);
    }

    public void UnassignControllable(IDevice device, IControllable controllable)
    {
        if (device.Slaves.Contains(controllable))
        {
            device.Slaves.Remove(controllable);
        }
        else
        {
            logger.Warning("Couldn't remove controllable from device (it wasn't listed)");
        }
    }

    public void AssignPlayer(IPlayer player, IDevice device)
    {
        device.Owner = player;
    }

    public void ConnectDevice(IDevice device)
    {
        devices.Add(device);
    }

    public void DisconnectDevice(IDevice device)
    {
        devices.Remove(device);
    }

    public List<IDevice> GetConnectedDevices()
    {
        return devices;
    }

    public IDevice GetDeviceState(Guid deviceId)
    {
        var result = devices.Find(x => x.Id == deviceId);
        if (result == null)
        {
            logger.Warning("Device with id " + deviceId.ToString() + " is not connected.");
        }
        return result;
    }
}
