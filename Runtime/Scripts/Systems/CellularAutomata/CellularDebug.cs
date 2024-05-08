using UnityEngine;

public class CellularDebug : MonoBehaviour
{
    public float brushSize = 3;
    public CellMaterial mouseSpawnMaterial = CellMaterial.Dirt;

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
                    var newCell = new Cell(mouseSpawnMaterial) { blocksLight = true };

                    CellularAutomata.instance.SetCell(globalPixelPosition, newCell);
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
                    CellularAutomata.instance.SetCell(globalPixelPosition, new Cell(CellMaterial.None));
                }
            }
        }
    }
}

