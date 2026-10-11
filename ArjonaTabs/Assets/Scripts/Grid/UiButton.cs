using TMPro;
using UnityEngine;

public class UiButton : MonoBehaviour
{
    [SerializeField] private GameObject bluePrefab;
    [SerializeField] private GameObject redPrefab;

    [SerializeField] private PlacementSystem placementSystemBlue;
    [SerializeField] private PlacementSystem placementSystemRed;
    
    [SerializeField] private string unitType;
    [SerializeField] private TMP_Text costText;

    private void Start()
    {
        if (costText == null || ExcelManager.Instance == null)
        {
            return;
        }

        if (ExcelManager.Instance.IsLoaded)
        {
            UpdateCost();
        }
        else
        {
            ExcelManager.Instance.OnDataLoaded += UpdateCost;
        }
    }

    private void OnDestroy()
    {
        if (ExcelManager.Instance != null)
        {
            ExcelManager.Instance.OnDataLoaded -= UpdateCost;
        }
    }

    public void SelectEnemy()
    {
        placementSystemBlue.StartPlacement(bluePrefab);
        placementSystemRed.StartPlacement(redPrefab);
    }

    private void UpdateCost()
    {
        ExcelManager.Instance.OnDataLoaded -= UpdateCost;

        if (ExcelManager.Instance.TryGetUnitData(unitType, out ExcelManager.UnitData data))
        {
            costText.text = data.costCurrency.ToString();
        }
    }
}
