using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PixelLight : MonoBehaviour
{
    public PixelLightType type;
    public float orientation = 0;
    [Range(0f, 360f)] public float arc = 360f;

    public Transform lookAtTarget = null;
    public float radiusInPixels = 32f;
    public float brightness = 1f;
    public Color color = Color.white;

    public float radiusInUnits => radiusInPixels / Global.pixelsPerUnit;

    public float blinkFrequency = 0;
    public float blinkAmount = 0;

    [HideInInspector] public float opacity = 1;

    float runtimeBrightness => brightness * opacity + Mathf.Sin(Time.time * blinkFrequency + phase) * blinkAmount;

    [HideInInspector] public float phase = 0;

    LDtkFields ldtkFields { get { if (_ldtkFields == null) { _ldtkFields = GetComponent<LDtkFields>(); } return _ldtkFields; } }
    LDtkFields _ldtkFields;

    public int numberOfRays = 360;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public LayerMask collisionLayer;
    Material pixelLightMaterial;

    float angle => lookAtTarget != null ? Vector2.SignedAngle(Vector2.right, lookAtTarget.transform.position - transform.position) : orientation;

    public bool debugRays = false;

    bool isDirty = true;

    private void Start()
    {
        phase = Random.value * 100;
        blinkFrequency += Random.Range(0.9f, 1.1f);
    }

    private void LateUpdate()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            pixelLightMaterial = null;
        }
        if (pixelLightMaterial == null)
        {
            var tempMaterial = new Material(Resources.Load<Material>("Shaders/PixelLight"));
            pixelLightMaterial = tempMaterial;
            meshRenderer.sharedMaterial = tempMaterial;
        }

        isDirty = true;

        if (isDirty)
        {
            switch (type)
            {
                case PixelLightType.Point:
                    meshFilter.mesh = CreatePointLightMesh();
                    break;
                case PixelLightType.Directional:
                    meshFilter.mesh = CreateDirectionalLightMesh();
                    break;
            }

            meshRenderer.sharedMaterial.SetFloat("_Radius", radiusInUnits);
            meshRenderer.sharedMaterial.SetFloat("_Brightness", runtimeBrightness);
            meshRenderer.sharedMaterial.SetColor("_Color", color);
            isDirty = false;
        }
    }

    Mesh CreatePointLightMesh(bool ignoreCollisions = false)
    {
        Vector3[] vertices = new Vector3[numberOfRays + 1];
        int[] triangles = new int[numberOfRays * 3];

        float angleStep = arc / numberOfRays;
        vertices[0] = Vector3.zero;

        var startAngle = angle - arc / 2f;
        var endAngle = startAngle + arc;

        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = endAngle - angleStep * i;
            Vector2 direction = AngleToVector2(currentAngle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radiusInUnits, ignoreCollisions ? 0 : collisionLayer);
            if (debugRays)
            {
                Debug.DrawLine(transform.position, hit.point, Color.yellow);
            }

            Vector2 hitPoint = hit.collider != null ? (Vector2)transform.position + hit.distance * direction
                                                   : (Vector2)transform.position + direction * radiusInUnits;
            vertices[i + 1] = transform.InverseTransformPoint(hitPoint);

            int baseIndex = i * 3;
            triangles[baseIndex] = 0;
            triangles[baseIndex + 1] = i + 1;
            triangles[baseIndex + 2] = i + 2;
        }

        triangles[numberOfRays * 3 - 3] = 0;
        triangles[numberOfRays * 3 - 2] = numberOfRays;
        triangles[numberOfRays * 3 - 1] = 1;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    Mesh CreateDirectionalLightMesh(bool ignoreCollisions = false)
    {
        Vector3[] vertices = new Vector3[(numberOfRays + 1)*2];
        int[] triangles = new int[numberOfRays * 6];

        vertices[0] = Vector3.zero;

        Vector2 direction = AngleToVector2(angle);
        Vector2 tangent = direction.LeftPerpendicular();
        float halfArc = arc / 2f;

        for (int i = 0; i < numberOfRays; i++)
        {
            var origin = (Vector2)transform.position + tangent * ((float)i / (float)numberOfRays - 0.5f) * halfArc;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, radiusInUnits, ignoreCollisions ? 0 : collisionLayer);

            Vector2 hitPoint = (Vector2)origin + direction * (hit.collider != null ? hit.distance : radiusInUnits);

            int baseIndex = 2 * i;
            vertices[baseIndex] = transform.InverseTransformPoint(origin);
            vertices[baseIndex + 1] = transform.InverseTransformPoint(hitPoint);

            if (debugRays)
            {
                Draw.Vector(origin, hitPoint, Color.yellow);
            }

            if (i > 0)
            {
                int baseTriangleIndex = (i - 1) * 6;
                triangles[baseTriangleIndex]     = baseIndex - 2;
                triangles[baseTriangleIndex + 1] = baseIndex;
                triangles[baseTriangleIndex + 2] = baseIndex - 1;
                triangles[baseTriangleIndex + 3] = baseIndex - 1;
                triangles[baseTriangleIndex + 4] = baseIndex;
                triangles[baseTriangleIndex + 5] = baseIndex + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void OnDrawGizmosSelected()
    {
        if (type == PixelLightType.Point)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radiusInUnits);
        }
    }

    Vector2 AngleToVector2(float angleInDegrees)
    {
        float radian = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public enum PixelLightType { Point, Directional }
}
