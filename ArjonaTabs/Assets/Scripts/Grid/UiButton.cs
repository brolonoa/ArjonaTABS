using UnityEngine;

public class UiButton : MonoBehaviour
{
    [SerializeField] private GameObject bluePrefab;
    [SerializeField] private GameObject redPrefab;

    [SerializeField] private PlacementSystem placementSystemBlue;
    [SerializeField] private PlacementSystem placementSystemRed;

    public void SelectEnemy()
    {
        placementSystemBlue.StartPlacement(bluePrefab);
        placementSystemRed.StartPlacement(redPrefab);
    }
}
