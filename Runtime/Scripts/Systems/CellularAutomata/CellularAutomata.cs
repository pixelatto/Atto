using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellularAutomata : MonoBehaviour
{

    public Vector2Int size = new Vector2Int(128, 72);
    public LayerMask layerMask;

    Buffer currentBuffer;

    SpriteRenderer spriteRenderer;
    Texture2D texture;

    public CellType debugCell;

    public Color32[] colors;

    void Start()
    {
        currentBuffer = new Buffer(size);
    }

    private void Update()
    {
        CheckTexture();
        Step();

        debugCell = currentBuffer.cells[0, 0];
        colors = currentBuffer.ToColorArray();
    }

    private void CheckTexture()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = this.GetOrAddComponent<SpriteRenderer>();
        }
        if (spriteRenderer.sprite == null)
        {
            texture = new Texture2D(size.x, size.y);
            texture.filterMode = FilterMode.Point;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    texture.SetPixel(i, j, Color.black * 0.1f); //TODO: Set to color clear
                }
            }
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, size.x, size.y), Vector2.one * 0.5f, 8f);
            ScanTerrain();
        }
    }

    private void ScanTerrain()
    {
        currentBuffer = new Buffer(size);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var worldPosition = new Vector2((i + 0.5f) / 8f, (j + 0.5f) / 8f);
                var hit = Physics2D.OverlapPoint(worldPosition, layerMask);
                if (hit != null)
                {
                    currentBuffer.SetValue(i, j, CellType.Solid);
                }
                else
                {
                    currentBuffer.SetValue(i, j, CellType.Empty);
                }

                var sandHit = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask("CellularAutomata"));
                if (sandHit != null)
                {
                    currentBuffer.SetValue(i, j, CellType.Sand);
                }
            }
        }
    }

    void Step()
    {
        UpdateBuffer();
    }

    private void UpdateBuffer()
    {
        texture.SetPixels32(currentBuffer.ToColorArray());
        texture.Apply();
    }

    public class Buffer
    {
        public CellType[,] cells;
        Vector2Int size;

        public Buffer(Vector2Int size)
        {
            this.size = size;
            cells = new CellType[size.x, size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    cells[i, j] = 0;
                }
            }
        }

        public void SetValue(int x, int y, CellType value)
        {
            cells[x, y] = value;
        }

        public Color32[] ToColorArray()
        {
            Color32[] result = new Color32[size.x * size.y];

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int index = j * size.x + i;
                    result[index] = Cell2Color(cells[i, j]);
                }
            }

            return result;
        }
    }

    public static Color32 Cell2Color(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Solid:    return Color.black;
            case CellType.Sand:     return Color.yellow;
            default:                return Color.clear;
        }
    }

    public enum CellType { Empty, Solid, Sand }
}
