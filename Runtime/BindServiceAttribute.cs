using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindService : Attribute
{

    public string accessDescriptor = "";
    public ServiceMode serviceMode;

    public BindService(string accessDescriptor, ServiceMode serviceMode = ServiceMode.Enabled)
    {
        this.accessDescriptor = accessDescriptor;
        this.serviceMode = serviceMode;
    }

}

public enum ServiceMode { Undefined, Enabled, Hidden, Disabled }