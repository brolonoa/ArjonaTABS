using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
   private NavMeshAgent _agent;
   private EnemyController sm;
   private TeamMember currentTarget;
   
   private bool combatStarted = false;
   [SerializeField] private float rotationSpeed = 360f;

   private void Start()
   {
      _agent = GetComponent<NavMeshAgent>();
      sm = GetComponent<EnemyController>();
      InicialiceStats();
   }

   public void OnCombatStart()
   {
      InvokeRepeating("SearchNextTarget", 0, 0.3f);
      //_agent.destination = GameManager.Instance.destination.position;
      combatStarted = true;
      
   }

   private void SearchNextTarget()
   {
      currentTarget = BattleManager.Instance.FindClosestEnemy(sm.teamMember);
      sm.currentTarget = currentTarget;
      
   }
   private void InicialiceStats()
   {
      _agent.speed = sm.stats.speed;
      
   }

   private void Update()
   {
      
      if (!combatStarted || currentTarget == null)
      {
         return;
      }
      _agent.SetDestination(currentTarget.Position);
      
      LookAtTarget();
     
   }

   public bool IsInAttackRange()
   {
      if (currentTarget == null)
      {
         return false;
      }

      float distance = (currentTarget.Position - transform.position).sqrMagnitude;

      return distance <= sm.stats.range * sm.stats.range;
   }
   
   private void LookAtTarget()
   {
      Vector3 direction = currentTarget.Position - transform.position;
      direction.y = 0f;

      if (direction == Vector3.zero)
         return;

      Quaternion targetRotation = Quaternion.LookRotation(direction);

      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
   }

   public void StopAgent()
   {
      _agent.isStopped = true;
   }

   public void ResumeAgent()
   {
      _agent.isStopped = false;
   }
  
}
