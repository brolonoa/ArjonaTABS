using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MuteButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ToggleMusic();
        }
    }
}
