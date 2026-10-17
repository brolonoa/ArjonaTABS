using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   [SerializeField] GameObject uiPrefab;
   [SerializeField] TextMeshProUGUI blueMoneySpend;
   [SerializeField] TextMeshProUGUI redMoneySpend;
   
   private int blueMoneySpent;
   private int redMoneySpent;

   private void Start()
   {
      uiPrefab.SetActive(true);
      GameManager.Instance.OnCombatStart += HidePlacementUI;
      BattleManager.Instance.OnUnitPlaced += SetMoneySpended;
      blueMoneySpend.text = blueMoneySpent.ToString();
      redMoneySpend.text = redMoneySpent.ToString();
   }

   private void SetMoneySpended(int unitCost, bool isBlue)
   {
      Debug.Log(unitCost);
      if (isBlue)
      {
         blueMoneySpent += unitCost;
         blueMoneySpend.text = blueMoneySpent.ToString();
      }
      else
      {
         redMoneySpent += unitCost;
         redMoneySpend.text = redMoneySpent.ToString();
      }
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
      BattleManager.Instance.OnUnitPlaced -= SetMoneySpended;
   }
  
}
