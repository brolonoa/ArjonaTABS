using UnityEngine;

public class HealAttack : EnemyAttack
{
    public override void StartAttack()
    {
        sm.anim.SetTrigger("attack");
    }

    public void Heal()
    {
        if (sm.currentTarget == null)
            return;

        if (!sm.currentTarget.NeedsHealing)
            return;

        sm.currentTarget.Heal(sm.stats.heal);
    }
    
}
