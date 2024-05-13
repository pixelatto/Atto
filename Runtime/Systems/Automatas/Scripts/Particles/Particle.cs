using System;
using UnityEngine;

[System.Serializable]
public class Particle
{
    public Vector2 position;
    public Vector2 speed;
    public Vector2 accel;
    public Color color = Color.magenta;
    public CellMaterial material;
    public float gravityScale = 1;
    public float drag = 0;
    public bool isEthereal = false;

    float creationTime;
    public float currentLifetime => Time.time - creationTime;
    public float invulnerabilityTime = 0;

    public float lightEmission => materialProperties.lightEmission;
    public float lightRadius => materialProperties.lightRadius;

    public Vector2 previousPosition { get; private set; }

    CellMaterialProperties materialProperties { get { if (_materialProperties == null) { _materialProperties = CellularMaterials.instance.FindMaterial(material); }; return _materialProperties; } }
    CellMaterialProperties _materialProperties;

    public Particle(Vector2 worldPosition, CellMaterial cellMaterial)
    {
        this.material = cellMaterial;
        this.position = worldPosition;
        this.previousPosition = worldPosition;
        color = CellularMaterials.instance.FindMaterial(material).GetColor();
        creationTime = Time.time;
    }

    public Particle(Cell source, Vector2 worldPosition)
    {
        this.material = source.material;
        this.position = worldPosition;
        this.previousPosition = worldPosition;
        color = source.GetColor();
        creationTime = Time.time;
    }

    public void Integrate(ParticleAutomata automata, float deltaTime)
    {
        //Start
        previousPosition = position;
        accel += automata.globalGravity * gravityScale;
        accel -= speed * drag;

        //Integration
        speed += accel * deltaTime;
        position += speed * deltaTime;

        //End
        accel = Vector2.zero;
    }

    public bool Destroy()
    {
        if (currentLifetime <= invulnerabilityTime)
        {
            return false;
        }
        return true;
    }
}