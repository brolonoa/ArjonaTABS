using UnityEngine;

public class UiButton : MonoBehaviour
{  
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private PlacementSystem placementSystemBlue;
    [SerializeField] private PlacementSystem placementSystemRed;

    public void SelectEnemy()
    {
        placementSystemBlue.StartPlacement(enemyStats);
        placementSystemRed.StartPlacement(enemyStats);
    }
}
