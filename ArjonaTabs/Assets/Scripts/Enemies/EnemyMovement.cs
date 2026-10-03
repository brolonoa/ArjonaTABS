using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
   private NavMeshAgent _agent;

   private void Start()
   {
      _agent = GetComponent<NavMeshAgent>();
   }

   public void OnCombatStart()
   {
      _agent.destination = GameManager.Instance.destination.position;
   }
}
