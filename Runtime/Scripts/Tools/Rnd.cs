using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rnd
{
    public static int currentSeed { get; private set; } = 0;
    static System.Random random = new System.Random(currentSeed);

    public static void SetSeed(int seed)
    {
        currentSeed = seed;
        random = new System.Random(seed);
    }

    public static float value => (float)random.NextDouble();


    public static int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    public static float Range(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    public static Vector2 insideUnitCircle
    {
        get
        {
            double angle = random.NextDouble() * Math.PI * 2;
            double radius = Math.Sqrt(random.NextDouble());
            return new Vector2((float)(radius * Math.Cos(angle)), (float)(radius * Math.Sin(angle)));
        }
    }

    public static Vector3 insideUnitSphere
    {
        get
        {
            double theta = random.NextDouble() * Math.PI * 2;
            double phi = Math.Acos(2 * random.NextDouble() - 1);
            double radius = Math.Pow(random.NextDouble(), 1.0 / 3.0);
            return new Vector3(
                (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
                (float)(radius * Math.Sin(phi) * Math.Sin(theta)),
                (float)(radius * Math.Cos(phi))
            );
        }
    }

    public static Quaternion rotation
    {
        get
        {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble() * Math.PI * 2;
            double u3 = random.NextDouble() * Math.PI * 2;

            double sqrt1MinusU1 = Math.Sqrt(1 - u1);
            double sqrtU1 = Math.Sqrt(u1);

            return new Quaternion(
                (float)(sqrt1MinusU1 * Math.Sin(u2)),
                (float)(sqrt1MinusU1 * Math.Cos(u2)),
                (float)(sqrtU1 * Math.Sin(u3)),
                (float)(sqrtU1 * Math.Cos(u3))
            );
        }
    }

    public static Quaternion rotationUniform
    {
        get
        {
            return rotation;
        }
    }
}
