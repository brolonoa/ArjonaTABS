using System;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
   private EnemyController sm;
   private float maxHealth;
   public float currentHealth;
   public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

   // Sent with the real amount of each hit and each heal, so damage numbers and flashes don't depend on frames
   public event Action<float> OnDamaged;
   public event Action<float> OnHealed;

   private void Start()
   {
      sm = GetComponent<EnemyController>();
      maxHealth = sm.stats.health;
      currentHealth = maxHealth;
   }

   public void TakeDamage(float damage)
   {
      float defense = sm != null && sm.stats != null ? sm.stats.defense : 0f;
      float reduction = Mathf.Clamp01(defense / 100f);
      float finalDamage = damage * (1f - reduction);
      currentHealth -= finalDamage;
      OnDamaged?.Invoke(finalDamage);
      if (currentHealth <= 0)
      {
         Destroy(transform.parent.gameObject);
      }
   }
   public void Heal(float amount)
   {
      float before = currentHealth;
      currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
      if (currentHealth > before)
      {
         OnHealed?.Invoke(currentHealth - before);
      }
   }
}
