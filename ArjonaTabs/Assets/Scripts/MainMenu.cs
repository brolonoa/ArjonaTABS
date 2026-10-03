using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Juego";
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (optionsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
