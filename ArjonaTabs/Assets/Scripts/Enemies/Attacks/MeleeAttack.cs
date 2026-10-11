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
    
    private IEnumerator AttackRoutine()
    {
        
        while (isAttacking)
        {
            Vector3 boxCenter = transform.position + transform.forward;

            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation
            );

            foreach (Collider hitCollider in hitColliders)
            {
                TeamMember target = hitCollider.GetComponentInParent<TeamMember>();

                if (target == null)
                {
                    continue;
                }

                if (target.CurrentTeam == GetComponentInParent<TeamMember>().CurrentTeam)
                {
                    continue;
                }
                if (!hitTargets.Add(target))
                {
                    continue;
                }

                target.TakeDamage(sm.stats.damage);
               
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
