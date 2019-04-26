using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindService : Attribute
{

    public string accessDescriptor = "";
    public ServiceMode serviceMode;
    public ServiceCaching serviceCaching;

    public BindService(string accessDescriptor, ServiceMode serviceMode = ServiceMode.Enabled, ServiceCaching serviceCaching = ServiceCaching.Static)
    {
        this.accessDescriptor = accessDescriptor;
        this.serviceMode = serviceMode;
        this.serviceCaching = serviceCaching;
    }

}

public enum ServiceMode { Undefined, Enabled, Hidden, Disabled }
public enum ServiceCaching { Undefined, Static, Dynamic }