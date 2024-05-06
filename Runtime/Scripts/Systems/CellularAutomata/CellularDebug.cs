using UnityEngine;

public class CellularDebug : MonoBehaviour
{
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
        int brushSize = 3;
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = CellularAutomata.WorldToPixelPosition(worldPosition);
        if (Input.GetMouseButton(0))
        {
            for (int i = -brushSize; i < brushSize; i++)
            {
                for (int j = -brushSize; j < brushSize; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(i, j);
                    var newCell = new Cell(mouseSpawnMaterial) { blocksLight = true };

                    CellularAutomata.instance.SetCell(globalPixelPosition, newCell);
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            for (int i = -brushSize; i < brushSize; i++)
            {
                for (int j = -brushSize; j < brushSize; j++)
                {
                    var globalPixelPosition = pixelPosition + new Vector2Int(i, j);
                    CellularAutomata.instance.SetCell(globalPixelPosition, new Cell(CellMaterial.None));
                }
            }
        }
    }
}

