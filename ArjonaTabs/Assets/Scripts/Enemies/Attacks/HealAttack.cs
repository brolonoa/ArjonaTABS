using UnityEngine;

public class HealAttack : EnemyAttack
{
    public override void StartAttack()
    {
        sm.anim.SetTrigger("attack");
    }

    public void Heal()
    {
        TeamMember target = sm.currentTarget;

        if (target == null)
            return;

        if (sm.teamMember.IsEnemy(target))
        {
            target.TakeDamage(sm.stats.damage);
            return;
        }

        if (!target.NeedsHealing)
            return;

        target.Heal(sm.stats.heal);
    }

}
