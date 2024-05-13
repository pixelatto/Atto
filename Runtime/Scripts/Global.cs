using System;
using UnityEngine;

public class Global
{
    public static float pixelsPerUnit = 8;
    public static Vector2Int roomPixelSize = new Vector2Int(128, 72);
    public static Vector2Int resolution = new Vector2Int(128, 72);
    public static float aspectRatio => (float)resolution.x / (float)resolution.y;

    public static Vector2 roomWorldSize => new Vector2(128 / pixelsPerUnit, 72 / pixelsPerUnit);
    public static float worldPixelSize => 1f / (float)pixelsPerUnit;

    public static LayerMask terrainMask = LayerMask.NameToLayer("Terrain");
    public static LayerMask backgroundMask = LayerMask.NameToLayer("Background");
    public static LayerMask foregroundMask = LayerMask.NameToLayer("Foreground");
    public static LayerMask cloudsMask = LayerMask.GetMask("Clouds");

    public static PhysicsMaterial2D brakeMaterial = null;
    public static PhysicsMaterial2D stickyMaterial = Resources.Load<PhysicsMaterial2D>("PhysicsMaterials2D/Sticky");
    public static PhysicsMaterial2D slipperyMaterial = Resources.Load<PhysicsMaterial2D>("PhysicsMaterials2D/Icy");
    public static PhysicsMaterial2D unstableMaterial = Resources.Load<PhysicsMaterial2D>("PhysicsMaterials2D/Unstable");
    public static PhysicsMaterial2D rollMaterial = Resources.Load<PhysicsMaterial2D>("PhysicsMaterials2D/Rolling");


    public const float slowMomentumThreeshold = 0.5f;
    public const float mediumMomentumThreeshold = 2.5f;
    public const float fastMomentumThreeshold = 5f;
    public const float verticalMomentumThreeshold = 1f;

    public static Momentum ClassifyMomentum(float speed)
    {
        Momentum result = Momentum.None;

        if (speed > fastMomentumThreeshold)
        {
            result = Momentum.Fast;
        }
        else if (speed > mediumMomentumThreeshold)
        {
            result = Momentum.Medium;
        }
        else if (speed > slowMomentumThreeshold)
        {
            result = Momentum.Slow;
        }
        else
        {
            result = Momentum.None;
        }
        return result;
    }
}