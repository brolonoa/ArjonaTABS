using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MeleeAttack :EnemyAttack
{
    [SerializeField] private Vector3 boxSize;
    private HashSet<TeamMember> hitTargets = new HashSet<TeamMember>();
    
    private bool isAttacking=false;
    private Coroutine attackCoroutine;

    public override void StartAttack()
    {
        
        if (isAttacking)
        {
            return;
        }
        boxSize = new Vector3(boxSize.x, boxSize.y, sm.stats.range);
        hitTargets.Clear();
        sm.anim.SetTrigger("attack");
        isAttacking = true;
        
    }
    
    public void EnableDamage()
    {
        attackCoroutine = StartCoroutine(AttackRoutine());
    }
    public void DisableDamage()
    {
        isAttacking = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
    
    // Single target: each swing damages only one enemy. The current target is preferred;
    // if it isn't inside the hit box, the closest enemy inside it is hit instead
    private IEnumerator AttackRoutine()
    {
        TeamMember self = GetComponentInParent<TeamMember>();

        while (isAttacking && hitTargets.Count == 0)
        {
            Vector3 boxCenter = transform.position + transform.forward;

            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation
            );

            TeamMember victim = null;
            float closestDistance = float.MaxValue;

            foreach (Collider hitCollider in hitColliders)
            {
                TeamMember target = hitCollider.GetComponentInParent<TeamMember>();

                if (target == null || target.CurrentTeam == self.CurrentTeam)
                {
                    continue;
                }

                if (target == sm.currentTarget)
                {
                    victim = target;
                    break;
                }

                float distance = (target.Position - transform.position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    victim = target;
                }
            }

            if (victim != null)
            {
                hitTargets.Add(victim);
                victim.TakeDamage(sm.stats.damage);
            }

            yield return null;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 boxCenter = transform.position + transform.forward;

        Gizmos.matrix = Matrix4x4.TRS(
            boxCenter,
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
    
}
