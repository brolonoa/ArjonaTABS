using System;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
   protected EnemyController sm;
   private void Start()
   {
      sm = GetComponent<EnemyController>();
   }

   public abstract void StartAttack();
}
