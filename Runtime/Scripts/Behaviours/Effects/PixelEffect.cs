using System;
using UnityEngine;

public class PixelEffect : MonoBehaviour
{
    public PixelCamera pixelCamera;
    public int spriteWidth => pixelCamera.pixelWidth;
    public int spriteHeight => pixelCamera.pixelHeight;
    public Texture2D spriteTexture;

    protected virtual void Start()
    {
        UpdateSprite();
    }

    protected virtual void Update()
    {
        TrackCamera();
    }

    protected void UpdateSprite()
    {
        spriteTexture = new Texture2D(spriteWidth, spriteHeight);
        spriteTexture.filterMode = FilterMode.Point;
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteWidth, spriteHeight), Vector2.one * 0.5f, 8f);
    }

    private void TrackCamera()
    {
        if (pixelCamera != null)
        {
            transform.position = new Vector3(pixelCamera.transform.position.x, pixelCamera.transform.position.y, transform.position.z);
        }
    }
}