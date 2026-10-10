
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private TeamGrid teamGrid;

    [SerializeField] private GameObject gridVisualization;

    private GameObject selectedEnemyPrefab;
    private GridData enemyData;

    private void Start()
    {
        StopPlacement();

        enemyData = new GridData();
    }

    public void StartPlacement(GameObject prefab)
    {
        StopPlacement();

        selectedEnemyPrefab = prefab;

        if (selectedEnemyPrefab == null)
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

        if (!inputManager.TryGetClickedObject(out GameObject hitObject))
        {
            return;
        }

        TeamGrid clickedTeamGrid = hitObject.GetComponentInParent<TeamGrid>();

        if (clickedTeamGrid != teamGrid)
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (!CheckPlacementValidity(gridPosition))
        {
            return;
        }

        GameObject newObject = Instantiate(selectedEnemyPrefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        TeamMember teamMember = newObject.GetComponent<TeamMember>();

        if (teamMember != null)
        {
            teamMember.SetTeam(teamGrid.Team);
        }

        enemyData.AddObjectAt(gridPosition, Vector2Int.one, newObject);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return enemyData.CanPlaceObjectAt(gridPosition, Vector2Int.one);
    }

    private void StopPlacement()
    {
        selectedEnemyPrefab = null;

        if (gridVisualization != null)
        {
            gridVisualization.SetActive(false);
        }

        if (inputManager != null)
        {
            inputManager.OnClick -= PlaceEnemy;
            inputManager.OnExit -= StopPlacement;
        }
    }

    private void Update()
    {
        if (selectedEnemyPrefab == null)
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

   
}

