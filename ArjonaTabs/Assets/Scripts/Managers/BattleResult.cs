using System.Collections;
using UnityEngine;

public class BattleResult : MonoBehaviour
{
   [SerializeField] private GameObject blueVictoryPanel;
   [SerializeField] private GameObject redVictoryPanel;
   [SerializeField] private float checkTime = 0.25f;

   private void Start()
   {
      blueVictoryPanel.SetActive(false);
      redVictoryPanel.SetActive(false);
      GameManager.Instance.OnCombatStart += OnCombatStart;
   }

   private void OnDestroy()
   {
      if (GameManager.Instance != null)
      {
         GameManager.Instance.OnCombatStart -= OnCombatStart;
      }
   }

   private void OnCombatStart()
   {
      StopAllCoroutines();
      StartCoroutine(CheckBattle());
   }

   private IEnumerator CheckBattle()
   {
      WaitForSeconds wait = new WaitForSeconds(checkTime);

      while (true)
      {
         yield return wait;

         int blueAlive = 0;
         int redAlive = 0;

         foreach (TeamMember unit in FindObjectsByType<TeamMember>(FindObjectsSortMode.None))
         {
            if (unit.CurrentTeam == TeamMember.Team.Blue)
            {
               blueAlive++;
            }
            else
            {
               redAlive++;
            }
         }

         if (blueAlive > 0 && redAlive > 0)
         {
            continue;
         }

         if (blueAlive > 0)
         {
            blueVictoryPanel.SetActive(true);
         }
         else if (redAlive > 0)
         {
            redVictoryPanel.SetActive(true);
         }

         yield break;
      }
   }
}
