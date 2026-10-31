using System.Collections;
using UnityEngine;

// Damage over time: the potion's damage is applied little by little instead of all at once
public class PoisonEffect : MonoBehaviour
{
    private const float TickTime = 0.5f;

    public static void Apply(TeamMember target, float totalDamage, float duration)
    {
        if (target == null || totalDamage <= 0)
        {
            return;
        }

        if (duration <= 0)
        {
            target.TakeDamage(totalDamage);
            return;
        }

        target.gameObject.AddComponent<PoisonEffect>().Begin(target, totalDamage, duration);
    }

    private void Begin(TeamMember target, float totalDamage, float duration)
    {
        StartCoroutine(Tick(target, totalDamage, duration));
    }

    private IEnumerator Tick(TeamMember target, float totalDamage, float duration)
    {
        int ticks = Mathf.Max(1, Mathf.RoundToInt(duration / TickTime));
        float perTick = totalDamage / ticks;

        for (int i = 0; i < ticks; i++)
        {
            if (target == null)
            {
                yield break;
            }

            target.TakeDamage(perTick);

            if (i < ticks - 1)
            {
                yield return new WaitForSeconds(TickTime);
            }
        }

        Destroy(this);
    }
}
