using UnityEngine;

public class UiButton : MonoBehaviour
{  
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private PlacementSystem placementSystem;

    public void SelectEnemy()
    {
        placementSystem.StartPlacement(enemyStats);
    }
}
