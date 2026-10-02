using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] GameObject mouseIndicator, cellIndicator;
    [SerializeField] InputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private EnemyStats _enemyStats;
    private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = _enemyStats.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
           
            return;
        }
        gridVisualization.SetActive(true);
        inputManager.OnClick += PlaceEnemy;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceEnemy()
    {
        if (inputManager.IsPointerOverUIObject())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        GameObject newObject= Instantiate(_enemyStats.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
    }
    private void StopPlacement()
    {
        
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        inputManager.OnClick -= PlaceEnemy;
        inputManager.OnExit -= StopPlacement;
    }
    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
