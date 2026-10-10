
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

        if (!inputManager.TryGetClickedObject(out GameObject hitObject))
        {
            return;
        }

        TeamGrid teamGrid = hitObject.GetComponentInParent<TeamGrid>();

        if (teamGrid == null)
        {
            return;
        }

        if (teamGrid.gameObject != grid.gameObject)
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

        GameObject prefabToPlace;

        if (teamGrid.Team == TeamMember.Team.Blue)
        {
            prefabToPlace = selectedEnemy.BluePrefab;
        }
        else
        {
            prefabToPlace = selectedEnemy.RedPrefab;
        }

        GameObject newObject = Instantiate(prefabToPlace);

        TeamMember teamMember =
            newObject.GetComponent<TeamMember>();

        if (teamMember == null)
        {
            Debug.LogError(
                $"El prefab {newObject.name} no tiene TeamMember."
            );

            Destroy(newObject);
            return;
        }

        teamMember.SetTeam(teamGrid.Team);

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

        if (inputManager != null)
        {
            inputManager.OnClick -= PlaceEnemy;
            inputManager.OnExit -= StopPlacement;
        }
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

