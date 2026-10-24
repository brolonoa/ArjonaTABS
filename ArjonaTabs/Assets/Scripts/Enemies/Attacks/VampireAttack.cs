using UnityEngine;

// Hits its enemy target from range and steals life for itself (the "lifeSteal" stat)
public class VampireAttack : EnemyAttack
{
    public override void StartAttack()
    {
        sm.anim.SetTrigger("attack");
    }

    public void Hit()
    {
        TeamMember target = sm.currentTarget;

        if (target == null || !sm.teamMember.IsEnemy(target))
            return;

        float targetBefore = target.HealthPercentage;
        float selfBefore = sm.teamMember.HealthPercentage;

        target.TakeDamage(sm.stats.damage);
        sm.teamMember.Heal(sm.stats.lifeSteal);

        Debug.Log($"[Vampiro] {sm.teamMember.name} -> {target.name} | daño {sm.stats.damage} | vida objetivo {targetBefore:P0} -> {target.HealthPercentage:P0} | vida propia {selfBefore:P0} -> {sm.teamMember.HealthPercentage:P0}");
        
    }

}
