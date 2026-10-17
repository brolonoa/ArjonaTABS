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

   private void OnEnable()
   {
      GameManager.Instance.OnCombatStart += ActivateEnemies;
   }

   private void Start()
   {
      _enemyMovement = GetComponent<EnemyMovement>();
      _attack = GetComponent<EnemyAttack>();

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
            if (stats.IsHealer && !currentTarget.NeedsHealing)
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
