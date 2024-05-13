using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularChunkCollider : MonoBehaviour
{
    public CellularColliderType cellularColliderType { get; private set; } public enum CellularColliderType { Main, Lights }

    Grid tilemapGrid;
    Tilemap tilemap;
    TilemapCollider2D tilemapCollider;
    CompositeCollider2D compositeCollider;
    TilemapRenderer tilemapRenderer;
    Rigidbody2D rb2d;

    static TileBase emptyTile;
    static TileBase fullTile;
    static TileBase ulTile;
    static TileBase urTile;
    static TileBase blTile;
    static TileBase brTile;

    CellularChunk chunk;

    public void InitChunkCollider(CellularChunk chunk, CellularColliderType cellularColliderType)
    {
        this.chunk = chunk;
        this.cellularColliderType = cellularColliderType;
        SetupTiles();
        BuildTilemap();
        Recalculate();
    }

    private void SetupTiles()
    {
        if (emptyTile == null) { emptyTile = Resources.Load<TileBase>("Tiles/Tile_Empty"); }
        if (fullTile == null) { fullTile = Resources.Load<TileBase>("Tiles/Tile_Full"); }
        if (ulTile == null) { ulTile = Resources.Load<TileBase>("Tiles/Tile_UL"); }
        if (urTile == null) { urTile = Resources.Load<TileBase>("Tiles/Tile_UR"); }
        if (blTile == null) { blTile = Resources.Load<TileBase>("Tiles/Tile_BL"); }
        if (brTile == null) { brTile = Resources.Load<TileBase>("Tiles/Tile_BR"); }
    }

    private void BuildTilemap()
    {
        GameObject tilemapObject = new GameObject("ChunkCollider_"+ cellularColliderType);
        tilemapObject.transform.parent = transform;
        tilemapObject.transform.localPosition = Vector3.zero;
        tilemapGrid = tilemapObject.GetOrAddComponent<Grid>();
        tilemap = tilemapObject.GetOrAddComponent<Tilemap>();
        tilemapCollider = tilemapObject.GetOrAddComponent<TilemapCollider2D>();
        compositeCollider = tilemapObject.GetOrAddComponent<CompositeCollider2D>();
        tilemapRenderer = tilemapObject.GetOrAddComponent<TilemapRenderer>();
        rb2d = tilemapObject.GetOrAddComponent<Rigidbody2D>();
        switch (cellularColliderType)
        {
            case CellularColliderType.Main:
                tilemapObject.layer = LayerMask.NameToLayer("Terrain");
                break;
            case CellularColliderType.Lights:
                tilemapObject.layer = LayerMask.NameToLayer("Lighting");
                break;
        }
        tilemapRenderer.enabled = false;
        tilemapGrid.cellSize = new Vector3(1f / Global.pixelsPerUnit, 1f / Global.pixelsPerUnit, 1);
        tilemapCollider.usedByComposite = true;
        rb2d.bodyType = RigidbodyType2D.Static;
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }

    public void Recalculate()
    {
        bool current = false, top = false, bottom = false, left = false, right = false;

        for (int i = 0; i < Global.roomPixelSize.x; i++)
        {
            for (int j = 0; j < Global.roomPixelSize.y; j++)
            {
                var currentPosition = new Vector2Int(i, j);
                var currentTilePosition = new Vector3Int(i, j, 0);
                var currentCell = chunk[currentPosition.x, currentPosition.y];
                switch (cellularColliderType)
                {
                    case CellularColliderType.Main:
                        current = currentCell.IsSolid();
                        break;
                    case CellularColliderType.Lights:
                        current = currentCell.blocksLight;
                        break;
                }
                var resultTile = emptyTile;
                if (current)
                {
                    var topCell = chunk[currentPosition.x, currentPosition.y + 1];
                    var bottomCell = chunk[currentPosition.x, currentPosition.y - 1];
                    var leftCell = chunk[currentPosition.x - 1, currentPosition.y];
                    var rightCell = chunk[currentPosition.x + 1, currentPosition.y];

                    switch (cellularColliderType)
                    {
                        case CellularColliderType.Main:
                            top = topCell.IsSolid();
                            bottom = bottomCell.IsSolid();
                            left = leftCell.IsSolid();
                            right = rightCell.IsSolid();
                            break;
                        case CellularColliderType.Lights:
                            top = topCell.blocksLight;
                            bottom = bottomCell.blocksLight;
                            left = leftCell.blocksLight;
                            right = rightCell.blocksLight;
                            break;
                    }
                    int neighbourCount = (top ? 1 : 0) + (bottom ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
                    switch (neighbourCount)
                    {
                        case 4:
                            resultTile = fullTile;
                            break;
                        case 3:
                            resultTile = fullTile;
                            break;
                        case 2:
                            if (top && right) { resultTile = blTile; }
                            else if (right && bottom) { resultTile = ulTile; }
                            else if (bottom && left) { resultTile = urTile; }
                            else if (left && top) { resultTile = brTile; }
                            break;
                        case 1:
                            resultTile = emptyTile;
                            break;
                        case 0:
                            resultTile = emptyTile;
                            break;
                    }
                    if (neighbourCount == 2)
                    {
                        bool isBorder = i == 0 || j == 0 || i == Global.roomPixelSize.x - 1 || j == Global.roomPixelSize.y - 1;
                        if (isBorder)
                        {
                            resultTile = fullTile;
                        }
                    }
                }
                else
                {
                    resultTile = emptyTile;
                }
                tilemap.SetTile(currentTilePosition, resultTile);
            }
        }
        compositeCollider.GenerateGeometry();
    }
}