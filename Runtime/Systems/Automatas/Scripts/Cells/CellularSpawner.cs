using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularSpawner : MonoBehaviour
{
    public CellMaterial cellMaterial = CellMaterial.Water;
    public int spawnAmount = 3;
    public float spawnRate = 0.1f;
    public SpawnType spawnType; public enum SpawnType { AsParticle, AsCell }
    public SpawnShape spawnShape => ((circleCollider != null) ? SpawnShape.Circle : ((boxCollider != null) ? SpawnShape.Box : SpawnShape.None));
    public enum SpawnShape { None, Circle, Box }

    float lastSpawnTime = 0;

    public BoxCollider2D boxCollider { get { if (boxCollider_ == null) { boxCollider_ = GetComponentInChildren<BoxCollider2D>(); }; return boxCollider_; } }
    private BoxCollider2D boxCollider_;

    public CircleCollider2D circleCollider { get { if (circleCollider_ == null) { circleCollider_ = GetComponent<CircleCollider2D>(); }; return circleCollider_; } }
    private CircleCollider2D circleCollider_;


    void Update()
    {
        if (Time.time - lastSpawnTime > spawnRate)
        {
            lastSpawnTime = Time.time;
            Spawn();
        }
    }

    void Spawn()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            var worldSpawnPosition = transform.position;
            Vector2? spawnPosition = null;
            switch (spawnShape)
            {
                case SpawnShape.Circle:
                    spawnPosition = (Vector2)circleCollider.bounds.center + Random.insideUnitCircle * circleCollider.radius;
                    break;
                case SpawnShape.Box:
                    spawnPosition = RandomBoxColliderPosition();
                    break;
            }
            if (spawnPosition != null)
            {
                SpawnItem((Vector2)spawnPosition, cellMaterial);
            }
            else
            {
                //Debug.LogWarning("Can't spawn", gameObject);
            }
        }
    }

    private void SpawnItem(Vector2 worldPosition, CellMaterial cellMaterial)
    {
        switch (spawnType)
        {
            case SpawnType.AsCell:
                CellularAutomata.instance.CreateCellIfEmpty(CellularAutomata.WorldToPixelPosition(worldPosition), cellMaterial);
                break;
            case SpawnType.AsParticle:
                ParticleAutomata.instance.CreateParticle(worldPosition, cellMaterial);
                break;
        }
    }

    Vector2 RandomBoxColliderPosition()
    {
        Bounds bounds = boxCollider.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }
}
