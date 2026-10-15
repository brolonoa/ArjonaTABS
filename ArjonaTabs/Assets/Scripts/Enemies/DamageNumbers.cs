using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
   [SerializeField] private EnemyHp enemyHp;
   [SerializeField] private float height = 2.6f;
   [SerializeField] private Color damageColor = new Color(1f, 0.25f, 0.15f);
   [SerializeField] private Color healColor = new Color(0.35f, 1f, 0.35f);
   [SerializeField] private float fontSize = 6f;
   [SerializeField] private float lifeTime = 0.9f;
   [SerializeField] private float riseSpeed = 1.2f;
   [SerializeField] private float randomOffset = 0.3f;

   private float lastHealth;
   private bool initialized;

   private void Update()
   {
      if (!initialized)
      {
         lastHealth = enemyHp.currentHealth;
         initialized = true;
         return;
      }

      CheckHealth();
   }

   private void OnDestroy()
   {
      if (initialized && gameObject.scene.isLoaded)
      {
         CheckHealth();
      }
   }

   private void CheckHealth()
   {
      float difference = lastHealth - enemyHp.currentHealth;
      lastHealth = enemyHp.currentHealth;

      int amount = Mathf.RoundToInt(Mathf.Abs(difference));
      if (amount == 0)
      {
         return;
      }

      Vector3 position = transform.position + Vector3.up * height;
      position += new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));

      if (difference > 0)
      {
         FloatingDamageText.Spawn(position, amount.ToString(), damageColor, fontSize, lifeTime, riseSpeed);
      }
      else
      {
         FloatingDamageText.Spawn(position, "+" + amount, healColor, fontSize, lifeTime, riseSpeed);
      }
   }
}
