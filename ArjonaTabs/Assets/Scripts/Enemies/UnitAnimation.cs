using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
   private static readonly int SpeedHash = Animator.StringToHash("Speed");

   [SerializeField] private Animator anim;
   [SerializeField] private NavMeshAgent agent;
   [SerializeField] private float smoothing = 10f;

   private float speed;

   private void Update()
   {
      if (anim == null || agent == null)
      {
         return;
      }

      bool moving = agent.enabled && agent.isOnNavMesh && !agent.isStopped;
      float target = moving ? agent.velocity.magnitude : 0f;

      speed = Mathf.Lerp(speed, target, smoothing * Time.deltaTime);
      anim.SetFloat(SpeedHash, speed);
   }
}
