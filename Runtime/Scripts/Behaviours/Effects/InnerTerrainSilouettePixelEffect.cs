using System;
using UnityEngine;

[ExecuteAlways]
public class InnerTerrainSilouettePixelEffect : PixelEffect
{
    public LayerMask collisionLayer;

    public bool reverse = false;

    public bool isDirty = true;

    public bool updateAlways = false;

    Vector3Int lastIntPosition = Vector3Int.zero;

    bool isLeftBound, isBottomBound, isRightBound, isTopBound;

    override protected void Update()
    {
        base.Update();
        FixPosition();
        if (isDirty || updateAlways)
        {
            UpdateSprite();
            Refresh();
            isDirty = false;
        }
    }

    private void FixPosition()
    {
        var intPosition = new Vector3Int(
            Mathf.RoundToInt(transform.position.x * 8),
            Mathf.RoundToInt(transform.position.y * 8),
            Mathf.RoundToInt(transform.position.z * 8)
        );
        if (intPosition != lastIntPosition)
        {
            lastIntPosition = intPosition;
            transform.position = new Vector3(
                (float)intPosition.x / 8f,
                (float)intPosition.y / 8f,
                (float)intPosition.z / 8f
            );
            isDirty = true;
        }
    }

    void Refresh()
    {
        for (int x = -spriteWidth / 2; x < spriteWidth / 2; x++)
        {
            for (int y = -spriteHeight / 2; y < spriteHeight / 2; y++)
            {
                float worldX = transform.position.x + ((x + 0.5f) * pixelCamera.worldPixelSize);
                float worldY = transform.position.y + ((y + 0.5f) * pixelCamera.worldPixelSize);

                bool hit = CheckForCollision(worldX, worldY);

                if (reverse) { hit = !hit; }

                spriteTexture.SetPixel(x + spriteWidth / 2, y + spriteHeight / 2, !hit ? Color.clear : Color.white);
            }
        }

        spriteTexture.Apply();
    }

    bool CheckForCollision(float centerX, float centerY)
    {
        float boundsMargin = 3f;

        isLeftBound = centerX < transform.position.x - 8f + boundsMargin * pixelCamera.worldPixelSize;
        isBottomBound = centerY < transform.position.y - 4.5f + boundsMargin * pixelCamera.worldPixelSize;
        isRightBound = centerX > transform.position.x + 8f - boundsMargin * pixelCamera.worldPixelSize;
        isTopBound = centerY > transform.position.y + 4.5f - boundsMargin * pixelCamera.worldPixelSize;

        Vector2 checkPoint;

        //checkPoint = FitVector(-1, 1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(0, 1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(1, 1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }

        //checkPoint = FitVector(-1, 0); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        checkPoint = FitVector(0, 0); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(1, 0); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }

        //checkPoint = FitVector(-1, -1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(0, -1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(1, -1); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }

        //checkPoint = FitVector(0, 2); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(0, 3); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(2, 0); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(-2, 0); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }
        //checkPoint = FitVector(0, -2); if (!Physics2D.OverlapCircle(new Vector2(centerX + checkPoint.x * pixelCamera.worldPixelSize, centerY + checkPoint.y * pixelCamera.worldPixelSize), pixelCamera.worldPixelSize * 0.25f, collisionLayer)) { return false; }

        return true;
    }

    Vector2 FitVector(int x, int y)
    {
        var fitX = Mathf.Clamp(x, isLeftBound ? 0 : x, isRightBound ? 0 : x);
        var fitY = Mathf.Clamp(y, isBottomBound ? 0 : y, isTopBound ? 0 : y);
        return new Vector2(fitX, fitY);
    }
}
