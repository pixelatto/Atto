using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindService : Attribute
{
    
    public string customAccessName;
    public ServiceMode serviceMode;
    public ServiceCaching serviceCaching;

    public BindService(ServiceMode serviceMode = ServiceMode.Enabled, ServiceCaching serviceCaching = ServiceCaching.Static, string customAccessName = "")
    {
        this.serviceMode = serviceMode;
        this.serviceCaching = serviceCaching;
        this.customAccessName = customAccessName;
    }

}

public enum ServiceMode { Undefined, Enabled, Hidden, Disabled }
public enum ServiceCaching { Undefined, Static, Dynamic }