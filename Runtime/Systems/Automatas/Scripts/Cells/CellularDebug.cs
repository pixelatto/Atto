using System.Linq;
using UnityEngine;

public class CellularDebug : MonoBehaviour
{
    [Header("Global")]
    public CellMaterial mouseSpawnMaterial = CellMaterial.Dirt;
    public float brushSize = 3;
    public bool randomColors = false;

    [Header("Particles")]
    public bool spawnAsParticles = false;
    public float particleSpawnSpeed = 5;

    private Texture2D paletteTexture;
    private Transform paletteTransform;

    private void Start()
    {
        // Asumiendo que la paleta de colores es un sprite en un GameObject llamado "ColorPalette"
        var paletteGO = GameObject.Find("ColorPalette");
        if (paletteGO == null)
        {
            return;
        }

        var spriteRenderer = paletteGO.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        paletteTexture = spriteRenderer.sprite.texture;
        paletteTransform = paletteGO.transform;
        FilterPaletteTexture(0.01f);
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            Vector3 mousePosition = Input.mousePosition;
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (IsClickOnColorPalette(mousePosition))
                {
                    PickMaterial(mousePosition);
                }
                else
                {
                    DrawMaterial();
                }
            }
        }
    }

    private void PickMaterial(Vector3 mousePosition)
    {
        Color clickedColor = GetColorAtMousePosition(mousePosition);
        mouseSpawnMaterial = GetMaterialFromColor(clickedColor);
    }

    private bool IsClickOnColorPalette(Vector3 mousePosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.name == "ColorPalette")
        {
            return true;
        }
        return false;
    }

    private Color GetColorAtMousePosition(Vector3 mousePosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2D, Vector2.zero);

        if (hit.collider != null)
        {
            float normalizedHitX = hit.point.x / (Camera.main.orthographicSize * Camera.main.aspect * 2f);
            float pixelX = normalizedHitX * paletteTexture.width;
            float pixelY = 0.5f * paletteTexture.height;

            Color color = paletteTexture.GetPixel((int)pixelX, (int)pixelY);
            return color;
        }

        return Color.white; // Default color if no hit
    }

    private CellMaterial GetMaterialFromColor(Color color)
    {
        float minDistance = float.MaxValue;
        CellMaterial closestMaterial = CellMaterial.Empty;

        foreach (var materialProperty in CellularMaterials.instance.materials)
        {
            float distance = ColorDistance(color, materialProperty.identifierColor);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestMaterial = materialProperty.cellMaterial;
            }
        }

        return closestMaterial;
    }

    private float ColorDistance(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;
        return rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;
    }

    private void FilterPaletteTexture(float threshold)
    {
        // Crear una copia de la textura de la paleta
        Texture2D filteredTexture = new Texture2D(paletteTexture.width, paletteTexture.height, paletteTexture.format, false);
        filteredTexture.filterMode = FilterMode.Point;
        Color[] originalPixels = paletteTexture.GetPixels();
        Color[] filteredPixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++)
        {
            Color currentColor = originalPixels[i];
            if (IsColorInMaterials(currentColor, threshold))
            {
                filteredPixels[i] = currentColor;
            }
            else
            {
                filteredPixels[i] = Color.clear; // Color transparente para eliminar
            }
        }

        filteredTexture.SetPixels(filteredPixels);
        filteredTexture.Apply();

        // Asignar la textura filtrada al sprite renderer de la paleta
        var paletteGO = GameObject.Find("ColorPalette");
        if (paletteGO != null)
        {
            var spriteRenderer = paletteGO.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = Sprite.Create(filteredTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), Global.pixelsPerUnit);
            }
        }
    }

    private bool IsColorInMaterials(Color color, float threshold)
    {
        foreach (var materialProperty in CellularMaterials.instance.materials)
        {
            float distance = ColorDistance(color, materialProperty.identifierColor);
            if (distance <= threshold)
            {
                return true;
            }
        }
        return false;
    }

    private void DrawMaterial()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = CellularAutomata.WorldToPixelPosition(worldPosition);

        if (Input.GetMouseButton(0))
        {
            for (float i = -brushSize * 0.5f; i < brushSize * 0.5f; i++)
            {
                for (float j = -brushSize * 0.5f; j < brushSize * 0.5f; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(Mathf.FloorToInt(i), Mathf.FloorToInt(j));

                    var newCell = CellularAutomata.instance.CreateCellIfEmpty(globalPixelPosition, mouseSpawnMaterial);
                    if (newCell != null && randomColors)
                    {
                        newCell.overrideColor = new Color(Random.value, Random.value, Random.value, 1);
                    }
                    if (spawnAsParticles)
                    {
                        var newParticle = PixelParticles.instance.CellToParticle(newCell, globalPixelPosition);
                        newParticle.speed = Random.insideUnitCircle * particleSpawnSpeed;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            for (float i = -brushSize * 0.5f; i < brushSize * 0.5f; i++)
            {
                for (float j = -brushSize * 0.5f; j < brushSize * 0.5f; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(Mathf.FloorToInt(i), Mathf.FloorToInt(j));
                    CellularAutomata.instance.DestroyCell(globalPixelPosition);
                }
            }
        }
    }
}
