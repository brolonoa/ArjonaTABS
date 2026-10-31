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

   // Attack states of the unit animators (soldier, tank, ranged, vampire)
   private static readonly int[] AttackStates =
   {
      Animator.StringToHash("Base Layer.MANOTAZO"),
      Animator.StringToHash("Base Layer.ESPADAZO"),
      Animator.StringToHash("Base Layer.THROW"),
      Animator.StringToHash("Base Layer.HEAL"),
   };

   // Starts the attack animation right away instead of waiting for the previous one to blend out,
   // so every unit attacks exactly every attackCooldown, as the Excel says
   protected void PlayAttackAnimation()
   {
      foreach (int state in AttackStates)
      {
         if (sm.anim.HasState(0, state))
         {
            sm.anim.CrossFadeInFixedTime(state, 0.1f, 0, 0f);
            return;
         }
      }

      sm.anim.SetTrigger("attack");
   }
}
