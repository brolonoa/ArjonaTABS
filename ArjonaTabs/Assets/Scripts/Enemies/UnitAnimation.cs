using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
   private static readonly int SpeedHash = Animator.StringToHash("Speed");

   [SerializeField] private Animator anim;
   [SerializeField] private NavMeshAgent agent;
   [SerializeField] private float smoothing = 10f;

   [Header("Attack timing")]
   [SerializeField] private string attackTag = "Attack";
   [Tooltip("Fraction of attackCooldown the attack animation is allowed to last")]
   [SerializeField] private float attackCooldownMargin = 0.9f;

   private EnemyController controller;
   private float speed;

   private void Awake()
   {
      controller = GetComponent<EnemyController>();
   }

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

      UpdateAttackSpeed();
   }

   // Attack animations longer than attackCooldown are sped up, so every unit attacks exactly
   // as often as the Excel says. Outside the attack the animator goes back to normal speed
   private void UpdateAttackSpeed()
   {
      AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
      float cooldown = controller != null && controller.stats != null ? controller.stats.attackCooldown : 0f;

      if (!state.IsTag(attackTag) || cooldown <= 0f)
      {
         anim.speed = 1f;
         return;
      }

      float allowedTime = cooldown * attackCooldownMargin;
      anim.speed = Mathf.Max(1f, state.length / allowedTime);
   }
}
