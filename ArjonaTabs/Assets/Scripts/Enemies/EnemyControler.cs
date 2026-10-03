using System;
using UnityEngine;

public class EnemyControler : MonoBehaviour
{
   [SerializeField] public EnemyStats stats;
   private EnemyMovement _enemyMovement;

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
