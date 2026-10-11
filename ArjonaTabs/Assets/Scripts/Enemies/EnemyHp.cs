using System;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
   private EnemyController sm;
   private float maxHealth;
   public float currentHealth;

   private void Start()
   {
      sm = GetComponent<EnemyController>();
      maxHealth = sm.stats.health;
      currentHealth = maxHealth;
   }

   public void TakeDamage(float damage)
   {
      currentHealth -= damage;
      if (currentHealth <= 0)
      {
         Destroy(transform.parent.gameObject);
      }
   }
}
