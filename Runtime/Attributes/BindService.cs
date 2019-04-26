using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindService : Attribute
{

    public string accessDescriptor;

    public BindService(string accessDescriptor)
    {
        this.accessDescriptor = accessDescriptor;
    }

}
