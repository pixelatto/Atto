using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutomata : MonoBehaviour
{
    public Vector2Int pixelSize = new Vector2Int(128, 72);
    public Vector2 globalGravity = new Vector2(0, -10);
    public List<Particle> particles = new List<Particle>();

    [Header("References")]
    public CellularAutomata cellularAutomata;
    public PixelCamera pixelCamera;

    SpriteRenderer spriteRenderer;
    Texture2D texture;
    Color32[] clearColors;
    List<Particle> markedToRemove = new List<Particle>();

    public static ParticleAutomata instance { get { if (instance_ == null) { instance_ = FindObjectOfType<ParticleAutomata>(); } return instance_; } }
    static ParticleAutomata instance_;

    public bool debug = false;

    private void Awake()
    {
        PrepareClearTexture();
        CheckTexture();
    }

    private void CheckTexture()
    {
        if (spriteRenderer == null)
        {
            var childObject = new GameObject("ParticleRasterizer");
            childObject.layer = Global.backgroundMask;
            childObject.transform.SetParent(transform);
            childObject.transform.localPosition = Vector3.zero;
            spriteRenderer = childObject.AddComponent<SpriteRenderer>();
        }
        if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture.width != pixelSize.x || spriteRenderer.sprite.texture.height != pixelSize.y)
        {
            texture = new Texture2D(pixelSize.x, pixelSize.y);
            texture.filterMode = FilterMode.Point;
            PrepareClearTexture();
            ClearTexture();
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, pixelSize.x, pixelSize.y), Vector2.one * 0.5f, Global.pixelsPerUnit);
        }
    }

    private void Update()
    {
        IntegrateParticles();
        CheckCollisions();
        RasterParticles();
        DrawDebugGizmos();
        RemoveParticles();
    }

    private void RemoveParticles()
    {
        foreach (var particle in markedToRemove)
        {
            particles.Remove(particle);
        }
        markedToRemove.Clear();
    }

    private void DrawDebugGizmos()
    {
        if (debug && Debug.isDebugBuild)
        {
            foreach (var particle in particles)
            {
                Draw.Circle(particle.position, 0.5f.PixelsToUnits(), Color.magenta, 8);
                Draw.Vector(particle.position, particle.position + particle.speed * Time.deltaTime, Color.cyan);
            }
        }
    }

    private void IntegrateParticles()
    {
        foreach (var particle in particles)
        {
            particle.Integrate(this, Time.deltaTime);
        }
    }

    private void CheckCollisions()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            var overlapCell = cellularAutomata.GetCell(CellularAutomata.WorldToPixelPosition(particle.position));
            
            if (!overlapCell.IsEmpty())
            {
                if (particle.isEthereal)
                {
                    DestroyParticle(particle);
                }
                else
                {
                    ParticleToCell(particle);
                }
            }

        }
    }

    private void RasterParticles()
    {
        ClearTexture();
        foreach (var particle in particles)
        {
            RasterParticle(particle);
        }
        texture.Apply();
    }

    private void RasterParticle(Particle particle)
    {
        var isInsideCamera = pixelCamera.worldRect.Contains(particle.position);
        if (isInsideCamera)
        {
            var pixelPosition = CellularAutomata.WorldToPixelPosition(particle.position);
            texture.SetPixel(pixelPosition.x, pixelPosition.y, particle.color);
        }
    }

    public Particle CreateParticle(Vector2 worldPosition, CellMaterial material)
    {
        var newParticle = new Particle(worldPosition, material);
        particles.Add(newParticle);
        return newParticle;
    }

    public bool DestroyParticle(Particle particle)
    {
        bool succeded = particle.Destroy();
        if (succeded)
        {
            markedToRemove.Add(particle);
        }
        return succeded;
    }

    public Particle CellToParticle(Cell cell, Vector2Int globalPixelPosition)
    {
        Particle newParticle = null;
        cellularAutomata.DestroyCell(globalPixelPosition);

        bool succeded = cellularAutomata.GetCell(globalPixelPosition).IsEmpty();

        if (succeded)
        {
            var worldPosition = CellularAutomata.PixelToWorldPosition(globalPixelPosition);
            newParticle = new Particle(cell, worldPosition);
            particles.Add(newParticle);
            Draw.Circle(worldPosition, 0.5f.PixelsToUnits(), Color.red, 8);
            Debug.DrawLine(worldPosition + Vector3.up * 1 / Global.pixelsPerUnit, worldPosition + Vector3.down * 1 / Global.pixelsPerUnit, Color.red, 1f);
            Debug.DrawLine(worldPosition + Vector3.left * 1 / Global.pixelsPerUnit, worldPosition + Vector3.right * 1 / Global.pixelsPerUnit, Color.red, 1f);
        }

        return newParticle;
    }

    public Cell ParticleToCell(Particle particle)
    {
        Cell newCell = CellularAutomata.emptyCell;
        Vector2Int? validPosition = null;
        var currentPixelPosition = CellularAutomata.WorldToPixelPosition(particle.position);
        var currentCell = cellularAutomata.GetCell(currentPixelPosition);
        var speedDirection = -particle.speed.normalized;
        if (currentCell.IsEmpty())
        {
            validPosition = currentPixelPosition;
        }
        else
        {
            for (float i = 0; i < 10; i++)
            {
                var candidatePosition = CellularAutomata.WorldToPixelPosition(particle.position + i.PixelsToUnits() * speedDirection);
                var candidateCell = cellularAutomata.GetCell(candidatePosition);
                if (candidateCell.IsEmpty())
                {
                    validPosition = candidatePosition;
                    break;
                }
            }
        }

        var destroySuccessful = DestroyParticle(particle);
        if (destroySuccessful)
        {
            if (validPosition != null)
            {
                newCell = cellularAutomata.CreateCell((Vector2Int)validPosition, particle.material);
            }
        }
        return newCell;
    }

    private void PrepareClearTexture()
    {
        clearColors = new Color32[pixelSize.x * pixelSize.y];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.clear;
        }
    }

    void ClearTexture()
    {
        texture.SetPixels32(clearColors);
    }
}
