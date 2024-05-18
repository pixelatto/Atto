using System.Collections.Generic;
using UnityEngine;
//public class TimeManager
//{
//    private float updateRate;
//    private float lastUpdateTime;

//    public TimeManager(float updateRate)
//    {
//        this.updateRate = updateRate;
//        lastUpdateTime = 0;
//    }

//    public bool ShouldUpdate()
//    {
//        if (Time.time - lastUpdateTime > updateRate)
//        {
//            lastUpdateTime = Time.time;
//            return true;
//        }
//        return false;
//    }
//}

public class StructuralConnectionManager
{
    private CellularAutomata automata;

    public StructuralConnectionManager(CellularAutomata automata)
    {
        this.automata = automata;
    }

    public void UpdateStructuralConnections(Vector2Int position)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
        bool isConnectedToStatic = false;

        toVisit.Enqueue(position);

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            if (!visited.Contains(current))
            {
                visited.Add(current);
                var currentCell = automata.GetCell(current);

                if (currentCell.material == CellMaterial.Empty)
                {
                    continue; // No consideres células vacías en la lógica de conexión
                }

                if (currentCell.movement == CellMovement.Static)
                {
                    isConnectedToStatic = true;
                }

                if (currentCell.movement == CellMovement.Structural)
                {
                    Vector2Int[] neighbors = {
                        new Vector2Int(current.x + 1, current.y),
                        new Vector2Int(current.x - 1, current.y),
                        new Vector2Int(current.x, current.y + 1),
                        new Vector2Int(current.x, current.y - 1)
                    };

                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            toVisit.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        // Actualizar todas las células visitadas basándose en el estado de conexión encontrado
        foreach (var cellPosition in visited)
        {
            var cell = automata.GetCell(cellPosition);
            if (cell.movement == CellMovement.Structural)
            {
                cell.isStructurallyConnected = isConnectedToStatic;
                cell.needsUpdate = false; // Marcar como no necesita actualización
            }
        }
    }

    public void PropagateStructuralUpdate(Vector2Int position)
    {
        Vector2Int[] neighbors = {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        foreach (var neighbor in neighbors)
        {
            var neighborCell = automata.GetCell(neighbor);
            if (neighborCell != null && neighborCell.movement == CellMovement.Structural)
            {
                neighborCell.needsUpdate = true; // Marcar las células adyacentes como que necesitan actualización
            }
        }
    }
}
