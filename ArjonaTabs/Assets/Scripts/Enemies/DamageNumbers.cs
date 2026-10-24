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
   [SerializeField] private float healExtraHeight = 0.5f;

   private void OnEnable()
   {
      if (enemyHp == null)
      {
         enemyHp = GetComponent<EnemyHp>();
      }

      if (enemyHp != null)
      {
         enemyHp.OnDamaged += ShowDamage;
         enemyHp.OnHealed += ShowHeal;
      }
   }

   private void OnDisable()
   {
      if (enemyHp != null)
      {
         enemyHp.OnDamaged -= ShowDamage;
         enemyHp.OnHealed -= ShowHeal;
      }
   }

   private void ShowDamage(float amount)
   {
      Spawn(amount.ToString("0.#"), damageColor, 0f);
   }

   private void ShowHeal(float amount)
   {
      Spawn("+" + amount.ToString("0.#"), healColor, healExtraHeight);
   }

   private void Spawn(string text, Color color, float extraHeight)
   {
      if (!gameObject.scene.isLoaded)
      {
         return;
      }

      Vector3 position = transform.position + Vector3.up * (height + extraHeight);
      position += new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
      FloatingDamageText.Spawn(position, text, color, fontSize, lifeTime, riseSpeed);
   }
}
