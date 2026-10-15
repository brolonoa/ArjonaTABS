using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
   [SerializeField] private string menuSceneName = "MenuPrincipal";

   public void LoadMenu()
   {
      SceneManager.LoadScene(menuSceneName);
   }
}
