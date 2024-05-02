using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PixelExtensions
{
    
    public static float PixelsToUnits(this int number)
    {
        return (float)number / 8f;
    }

    public static float PixelsToUnits(this float number)
    {
        return number / 8f;
    }
}
