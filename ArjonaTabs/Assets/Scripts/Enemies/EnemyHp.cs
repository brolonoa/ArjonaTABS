using System;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
   private EnemyController sm;
   private float maxHealth;
   private float baseMaxHealth;
   public float currentHealth;
   public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
   public float MaxHealth => maxHealth;

   [Tooltip("Healing above max health raises the max health, up to this multiple of the starting health")]
   [SerializeField] private float maxHealthGrowthLimit = 2f;

   // Sent with the real amount of each hit and each heal, so damage numbers and flashes don't depend on frames
   public event Action<float> OnDamaged;
   public event Action<float> OnHealed;

   private void Start()
   {
      sm = GetComponent<EnemyController>();
      maxHealth = sm.stats.health;
      baseMaxHealth = maxHealth;
      currentHealth = maxHealth;
   }

   public void TakeDamage(float damage)
   {
      float defense = sm != null && sm.stats != null ? sm.stats.defense : 0f;
      float reduction = Mathf.Clamp01(defense / 100f);
      float finalDamage = damage * (1f - reduction);
      currentHealth -= finalDamage;
      OnDamaged?.Invoke(finalDamage);
      if (currentHealth <= 0.1)
      {
         Destroy(transform.parent.gameObject);
      }
   }
   public void Heal(float amount)
   {
      // A unit that already died this frame can't heal back
      if (currentHealth <= 0)
         return;

      float before = currentHealth;
      currentHealth += amount;

      // Life steal is never wasted: whatever goes above max health raises the max health, up to the limit
      if (currentHealth > maxHealth)
      {
         maxHealth = Mathf.Min(currentHealth, baseMaxHealth * maxHealthGrowthLimit);
         currentHealth = Mathf.Min(currentHealth, maxHealth);
      }
      if (currentHealth > before)
      {
         OnHealed?.Invoke(currentHealth - before);
      }
   }
}
