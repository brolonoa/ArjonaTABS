using UnityEngine;

public class HealAttack : EnemyAttack
{
    public override void StartAttack()
    {
        sm.anim.SetTrigger("attack");
    }

    public void Heal()
    {
        //sm.currentTarget.
    }
}
