using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private GameObject gridVisualization;

    private EnemyStats selectedEnemy;

    private GridData enemyData;
    private bool isRemoving;

    private void Start()
    {
        StopPlacement();

        enemyData = new GridData();
    }

    public void StartPlacement(EnemyStats enemy)
    {
        StopPlacement();

        selectedEnemy = enemy;
        isRemoving = false;

        if (selectedEnemy == null)
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

        Vector3 mousePosition =
            inputManager.GetSelectedMapPosition();

        Vector3Int gridPosition =
            grid.WorldToCell(mousePosition);

        if (!CheckPlacementValidity(gridPosition))
        {
            return;
        }

        GameObject newObject =
            Instantiate(selectedEnemy.Prefab);

        newObject.transform.position =
            grid.CellToWorld(gridPosition);

        enemyData.AddObjectAt(
            gridPosition,
            selectedEnemy.Size,
            newObject
        );
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return enemyData.CanPlaceObjectAt(
            gridPosition,
            selectedEnemy.Size
        );
    }
    public void StartRemoval()
    {
        StopPlacement();

        isRemoving = true;

        gridVisualization.SetActive(true);

        inputManager.OnClick += RemoveEnemy;
        inputManager.OnExit += StopPlacement;
    }
    private void RemoveEnemy()
    {
        if (inputManager.IsPointerOverUIObject())
        {
            return;
        }

        Vector3 mousePosition =
            inputManager.GetSelectedMapPosition();

        Vector3Int gridPosition =
            grid.WorldToCell(mousePosition);

        enemyData.RemoveObjectAt(gridPosition);
    }

    private void StopPlacement()
    {
        selectedEnemy = null;
        isRemoving = false;

        if (gridVisualization != null)
        {
            gridVisualization.SetActive(false);
        }

        inputManager.OnClick -= PlaceEnemy;
        inputManager.OnClick -= RemoveEnemy;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedEnemy == null)
        {
            return;
        }

        Vector3 mousePosition =
            inputManager.GetSelectedMapPosition();

        Vector3Int gridPosition =
            grid.WorldToCell(mousePosition);

        mouseIndicator.transform.position =
            mousePosition;

        cellIndicator.transform.position =
            grid.CellToWorld(gridPosition);
    }
}