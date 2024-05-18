using System;
using UnityEngine;

public class CellularChunkRenderer : MonoBehaviour
{
    CellularChunk chunk;

    SpriteRenderer spriteRenderer;
    Texture2D texture;

    public CellRenderLayer layer { get; private set; }

    public void InitChunkRenderer(CellularChunk chunk, CellRenderLayer layer)
    {
        this.chunk = chunk;
        this.layer = layer;
        InitTexture();
    }

    private void InitTexture()
    {
        var childObject = gameObject.FindOrAddChild("ChunkRenderer_" + layer);
        childObject.layer = CellLayerToLayer(layer);
        childObject.transform.SetParent(transform);
        childObject.transform.localPosition = new Vector3(chunk.pixelSize.x / Global.pixelsPerUnit / 2f, chunk.pixelSize.y / Global.pixelsPerUnit / 2f, 0);
        spriteRenderer = childObject.GetOrAddComponent<SpriteRenderer>();
        texture = new Texture2D(chunk.pixelSize.x, chunk.pixelSize.y);
        texture.filterMode = FilterMode.Point;
        for (int i = 0; i < chunk.pixelSize.x; i++)
        {
            for (int j = 0; j < chunk.pixelSize.y; j++)
            {
                texture.SetPixel(i, j, Color.clear);
            }
        }
        texture.Apply();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, chunk.pixelSize.x, chunk.pixelSize.y), Vector2.one * 0.5f, Global.pixelsPerUnit);
        Draw();
    }

    int CellLayerToLayer(CellRenderLayer cellLayer)
    {
        switch (cellLayer)
        {
            case CellRenderLayer.Main:  return Global.terrainLayer;
            case CellRenderLayer.Back:  return Global.backgroundLayer;
            case CellRenderLayer.Front: return Global.foregroundLayer;
            default:                    return 0;
        }
    }

    public void Draw()
    {
        texture.SetPixels32(chunk.ChunkToColorArray(layer));
        texture.Apply();
    }
}