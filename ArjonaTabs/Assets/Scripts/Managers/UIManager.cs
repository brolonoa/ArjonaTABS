using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   [SerializeField] GameObject uiPrefab;

   private void Start()
   {
      uiPrefab.SetActive(true);
      GameManager.Instance.OnCombatStart += HidePlacementUI;
   }

  
   private void HidePlacementUI()
   {
      uiPrefab.SetActive(false);
   }
   private void ShowPlacementUI()
   {
   }

   private void OnDisable()
   {
      GameManager.Instance.OnCombatStart -= HidePlacementUI;
   }
}
