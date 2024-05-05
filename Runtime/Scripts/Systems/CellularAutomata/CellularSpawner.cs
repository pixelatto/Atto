using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularSpawner : MonoBehaviour
{
    public CellMaterial cellMaterial = CellMaterial.Water;
    public int spawnRadius = 3;
    public float spawnRate = 0.1f;

    float lastSpawnTime = 0;

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
        /*
        var pixelPosition = CellularAutomata.instance.currentViewport.WorldToPixelPosition(transform.position);
        for (float i = -spawnRadius/2f; i < spawnRadius/2f; i += 0.5f)
        {
            for (float j = -spawnRadius/2f; j < spawnRadius/2f; j += 0.5f)
            {
                CellularAutomata.instance.currentViewport.SetValue(pixelPosition + new Vector2Int(Mathf.RoundToInt(i), Mathf.RoundToInt(j)), new Cell(cellMaterial));
            }
        }
        */
        Debug.Log("Disabled");
    }
}
