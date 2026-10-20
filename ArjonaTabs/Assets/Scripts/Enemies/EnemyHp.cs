using System;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
   private EnemyController sm;
   private float maxHealth;
   public float currentHealth;
   public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

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
      currentHealth -= damage * (1f - reduction);
      if (currentHealth <= 0)
      {
         Destroy(transform.parent.gameObject);
      }
   }
   public void Heal(float amount)
   {
      currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
   }
}
