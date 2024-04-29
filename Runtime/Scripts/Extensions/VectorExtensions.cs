using UnityEngine;

public static class VectorExtensions
{

    public static Vector2 Project(this Vector2 vector, Vector2 projectionDirection)
    {
        return Vector3.Project(vector, projectionDirection);
    }

    public static Vector2 LeftPerpendicular(this Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }

    public static Vector2 RightPerpendicular(this Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x);
    }

    public static Vector2 AngleToVector(this Vector2 vector, float angle, float distance = 1f)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * distance;
    }



}