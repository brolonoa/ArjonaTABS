using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Switches the battle between normal and double speed
[RequireComponent(typeof(Button))]
public class SpeedButton : MonoBehaviour
{
    [SerializeField] private float fastSpeed = 2f;
    [SerializeField] private TMP_Text label;

    private bool fast;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
        UpdateLabel();
    }

    private void Toggle()
    {
        fast = !fast;
        Time.timeScale = fast ? fastSpeed : 1f;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label != null)
        {
            label.text = fast ? $"x{fastSpeed:0}" : "x1";
        }
    }

    // Leaving the scene (back to the menu) always goes back to normal speed
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
