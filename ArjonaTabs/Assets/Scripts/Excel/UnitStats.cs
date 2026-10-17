using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Header("Row name in Google Sheets (soldier, tank, ranged, healer)")]
    [SerializeField] private string unitType;

    [Header("Unit Parameters from Google Sheets")]
    public int health;
    public int damage;
    public float speed;
    public float attackCooldown;
    public float range;
    public int costCurrency;
    public float heal;

    public bool IsHealer => damage <= 0;

    private void Start()
    {
        if (ExcelManager.Instance == null)
        {
            return;
        }

        if (ExcelManager.Instance.IsLoaded)
        {
            ApplyStats();
        }
        else
        {
            ExcelManager.Instance.OnDataLoaded += ApplyStats;
        }
    }

    private void OnDestroy()
    {
        if (ExcelManager.Instance != null)
        {
            ExcelManager.Instance.OnDataLoaded -= ApplyStats;
        }
    }

    private void ApplyStats()
    {
        ExcelManager.Instance.OnDataLoaded -= ApplyStats;

        if (!ExcelManager.Instance.TryGetUnitData(unitType, out ExcelManager.UnitData data))
        {
            Debug.LogWarning($"{name}: no hay datos para '{unitType}' en el Excel.");
            return;
        }

        health = data.health;
        damage = data.damage;
        speed = data.speed;
        attackCooldown = data.attackCooldown;
        range = data.range;
        costCurrency = data.costCurrency;
        heal = data.heal;
    }
}
