using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
   [SerializeField] public UnitStats stats;
   public TeamMember teamMember;

   public Animator anim;
   
   private EnemyMovement _enemyMovement;
   private EnemyAttack _attack;
   private bool isEnable = false;
   private bool isAttackReady = true;
   public TeamMember currentTarget;

   private bool isSubscribed = false;

   private void OnEnable()
   {
      SubscribeToCombat();
   }

   private void OnDisable()
   {
      if (isSubscribed && GameManager.Instance != null)
      {
         GameManager.Instance.OnCombatStart -= ActivateEnemies;
      }
      isSubscribed = false;
   }

   private void Start()
   {
      _enemyMovement = GetComponent<EnemyMovement>();
      _attack = GetComponent<EnemyAttack>();
      SubscribeToCombat();
   }

   private void SubscribeToCombat()
   {
      if (isSubscribed || GameManager.Instance == null)
      {
         return;
      }

      GameManager.Instance.OnCombatStart += ActivateEnemies;
      isSubscribed = true;
   }

   private void ActivateEnemies()
   {
      _enemyMovement.OnCombatStart();
      isEnable = true;
   }

   private void Update()
   {
      if (!isEnable)
      {
         return;
      }

      if (isAttackReady)
      {
         
         if (_enemyMovement.IsInAttackRange())
         {
            if (stats.IsHealer && !teamMember.IsEnemy(currentTarget) && !currentTarget.NeedsHealing)
               return;
            _attack.StartAttack();
            StartCoroutine(AttackCooldown());
            isAttackReady=false;
         } 
         
      }
     
   }

   private IEnumerator AttackCooldown()
   {
      _enemyMovement.StopAgent();
      yield return new WaitForSeconds(stats.attackCooldown);
      isAttackReady = true;
      _enemyMovement.ResumeAgent();
   }
}
