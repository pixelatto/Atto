using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Scan
{
    public static RaycastHit2D[] RaycastArc(Vector3 origin, float centralAngle, float maxDistance, LayerMask layerMask, float arcAngle = 180f, int numRays = 10)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        Vector3 startPosition = origin;
        float startAngle = centralAngle - arcAngle / 2f;

        float angleIncrement = arcAngle / (numRays - 1);

        for (int i = 0; i < numRays; i++)
        {
            float currentAngle = startAngle + angleIncrement * i;
            Vector2 direction = new Vector2().AngleToVector(currentAngle, maxDistance);
            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, maxDistance, layerMask);
            if (hit)
            {
                hits.Add(hit);
            }
        }

        return hits.ToArray();
    }

    public static Collider2D[] NearbyObjects(Vector2 position, float radius, int layerMask)
    {
        return Physics2D.OverlapCircleAll(position, radius, layerMask);
    }
}
