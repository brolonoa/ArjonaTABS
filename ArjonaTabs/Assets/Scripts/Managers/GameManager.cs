using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
   public static GameManager Instance{get;private set;}
   public event Action OnCombatStart;
   public Transform destination;
   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
   }

   public void StartCombat()
   {
      OnCombatStart?.Invoke();
   }

   public void RemoveAll()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }
}
