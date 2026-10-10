
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private GameObject gridVisualization;
    
    private TeamMember.Team selectedTeam;

    private EnemyStats selectedEnemy;
    private GridData enemyData;

    private void Start()
    {
        StopPlacement();

        enemyData = new GridData();
    }

    public void StartPlacement(EnemyStats enemy)
    {
        StopPlacement();

        selectedEnemy = enemy;

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

    private void StopPlacement()
    {
        selectedEnemy = null;

        if (gridVisualization != null)
        {
            gridVisualization.SetActive(false);
        }

        inputManager.OnClick -= PlaceEnemy;
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
