using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(
        Vector3Int gridPosition,
        Vector2Int objectSize,
        GameObject placedObject)
    {
        List<Vector3Int> positionsToOccupy =
            CalculatePositions(gridPosition, objectSize);

        PlacementData data =
            new PlacementData(positionsToOccupy, placedObject);

        foreach (Vector3Int position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                throw new Exception(
                    $"Dictionary already contains this cell position {position}");
            }

            placedObjects[position] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(
        Vector3Int gridPosition,
        Vector2Int objectSize)
    {
        List<Vector3Int> positions = new();

        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                positions.Add(
                    gridPosition + new Vector3Int(x, 0, y)
                );
            }
        }

        return positions;
    }

    public bool CanPlaceObjectAt(
        Vector3Int gridPosition,
        Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy =
            CalculatePositions(gridPosition, objectSize);

        foreach (Vector3Int position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                return false;
            }
        }

        return true;
    }

    public GameObject GetPlacedObject(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            return null;
        }

        return placedObjects[gridPosition].PlacedObject;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            return;
        }

        PlacementData data = placedObjects[gridPosition];

        foreach (Vector3Int position in data.OccupiedPositions)
        {
            placedObjects.Remove(position);
        }

        UnityEngine.Object.Destroy(data.PlacedObject);
    }
}

public class PlacementData
{
    public List<Vector3Int> OccupiedPositions { get; private set; }

    public GameObject PlacedObject { get; private set; }

    public PlacementData(
        List<Vector3Int> occupiedPositions,
        GameObject placedObject)
    {
        OccupiedPositions = occupiedPositions;
        PlacedObject = placedObject;
    }
}