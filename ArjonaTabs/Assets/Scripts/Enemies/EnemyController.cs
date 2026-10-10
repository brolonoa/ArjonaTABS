using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
   [SerializeField] public UnitStats stats;
   private EnemyMovement _enemyMovement;
   public TeamMember teamMember;

   private void OnEnable()
   {
      GameManager.Instance.OnCombatStart += ActivateEnemies;
   }

   private void Start()
   {
      _enemyMovement = GetComponent<EnemyMovement>();
   }

   private void ActivateEnemies()
   {
      _enemyMovement.OnCombatStart();
   }
}
