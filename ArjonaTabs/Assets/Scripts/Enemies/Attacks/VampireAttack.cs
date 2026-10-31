using UnityEngine;

// Hits its enemy target from range and steals life for itself (the "lifeSteal" stat)
public class VampireAttack : EnemyAttack
{
    // Life steal waits for the end of the frame, so hits that land at the same time are fair for both teams
    private float pendingHeal;

    public override void StartAttack()
    {
        PlayAttackAnimation();
    }

    public void Hit()
    {
        TeamMember target = sm.currentTarget;

        if (target == null || !sm.teamMember.IsEnemy(target))
            return;

        target.TakeDamage(sm.stats.damage);
        pendingHeal += sm.stats.lifeSteal;
    }

    private void LateUpdate()
    {
        if (pendingHeal <= 0f)
            return;

        sm.teamMember.Heal(pendingHeal);
        pendingHeal = 0f;
    }

}
