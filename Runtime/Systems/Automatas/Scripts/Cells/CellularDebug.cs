using UnityEngine;

public class CellularDebug : MonoBehaviour
{
    [Header("Global")]
    public float brushSize = 3;
    public CellMaterial mouseSpawnMaterial = CellMaterial.Dirt;

    [Header("Particles")]
    public bool spawnAsParticles = false;
    public float particleSpawnSpeed = 5;

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            DebugControls();
        }
    }

    void DebugControls()
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

                    var newCell = CellularAutomata.instance.CreateCell(globalPixelPosition, mouseSpawnMaterial);
                    if (spawnAsParticles)
                    {
                        var newParticle = ParticleAutomata.instance.CellToParticle(newCell, globalPixelPosition);
                        newParticle.speed = Random.insideUnitCircle * particleSpawnSpeed;
                    }
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            for (float i = -brushSize*0.5f; i < brushSize*0.5f; i++)
            {
                for (float j = -brushSize*0.5f; j < brushSize*0.5f; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(Mathf.FloorToInt(i), Mathf.FloorToInt(j));
                    CellularAutomata.instance.DestroyCell(globalPixelPosition);
                }
            }
        }
    }
}

