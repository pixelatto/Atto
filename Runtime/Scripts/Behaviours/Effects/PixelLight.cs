using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PixelLight : MonoBehaviour
{
    public Color color = Color.white;
    public float overrideRadius = 0f;
    public float overrideBrightness = 0f;
    float radiusInUnits => (ldtkFields != null) ? (overrideRadius != 0 ? overrideRadius : ldtkFields.GetFloat("Radius")) : overrideRadius;
    public float brightness => (ldtkFields != null) ? (overrideBrightness != 0 ? overrideBrightness : ldtkFields.GetFloat("Brightness")) : overrideBrightness;
    public float radius => radiusInUnits / 8f;

    public float blinkFrequency = 0;
    public float blinkAmount = 0;

    [HideInInspector] public float phase = 0;

    DitherLightingPixelEffect ditherLightingPixelEffect;
    LDtkFields ldtkFields { get { if (_ldtkFields == null) { _ldtkFields = GetComponent<LDtkFields>(); } return _ldtkFields; } }
    LDtkFields _ldtkFields;

    public int numberOfRays = 360;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public LayerMask collisionLayer;
    public float lightPenetration = 0.25f;
    Material pixelLightMaterial;

    private void Start()
    {
        phase = Random.value * 100;
        blinkFrequency += Random.Range(0.9f, 1.1f);
    }

    private void Update()
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

        ScanAndCreateShadowMesh();

        if (ditherLightingPixelEffect == null)
        {
            ditherLightingPixelEffect = FindObjectOfType<DitherLightingPixelEffect>();
        }

        if (ditherLightingPixelEffect != null)
        {
            if (ditherLightingPixelEffect.pixelCamera.worldRect.Contains(transform.position))
            {
                Draw.Circle(transform.position, 0.5f, Color.yellow);
                ditherLightingPixelEffect.SubscribeLight(this);
            }
            else
            {
                Draw.Circle(transform.position, 0.5f, Color.red*0.25f);
                ditherLightingPixelEffect.UnsubscribeLight(this);
            }
        }
    }

    private void OnDestroy()
    {
        if (ditherLightingPixelEffect != null)
        {
            ditherLightingPixelEffect.UnsubscribeLight(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void ScanAndCreateShadowMesh()
    {
        Vector3[] vertices = new Vector3[numberOfRays + 1];
        int[] triangles = new int[numberOfRays * 3];

        float angleStep = 360f / numberOfRays;
        vertices[0] = Vector3.zero;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = -angleStep * i;
            Vector2 direction = AngleToVector2(angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radius, collisionLayer);
            var penetrationVector = hit.normal * lightPenetration;
            if (Vector2.Dot(direction, -penetrationVector) > 0) { penetrationVector = Vector2.zero; }
            Vector2 hitPoint = hit.collider != null ? (Vector2)transform.position + hit.distance * direction - penetrationVector
                                                   : (Vector2)transform.position + direction * radius;
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

        meshFilter.mesh = mesh;

        pixelLightMaterial.SetFloat("_Radius", radius);
    }

    Vector2 AngleToVector2(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
}
