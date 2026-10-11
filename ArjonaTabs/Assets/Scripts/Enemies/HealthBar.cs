using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
   [SerializeField] private EnemyHp enemyHp;
   [SerializeField] private EnemyController controller;
   [SerializeField] private RectTransform fill;
   [SerializeField] private Image fillImage;
   [SerializeField] private Color fullColor = new Color(0.3f, 0.85f, 0.25f);
   [SerializeField] private Color emptyColor = new Color(0.85f, 0.15f, 0.1f);

   private Camera cam;
   private float maxHealth;

   private void Start()
   {
      cam = Camera.main;
   }

   private void Update()
   {
      if (maxHealth <= 0)
      {
         maxHealth = controller != null && controller.stats != null && controller.stats.health > 0 ? controller.stats.health : enemyHp.currentHealth;
      }

      float percent = maxHealth > 0 ? Mathf.Clamp01(enemyHp.currentHealth / maxHealth) : 0;
      fill.anchorMax = new Vector2(percent, 1);
      //fillImage.color = Color.Lerp(emptyColor, fullColor, percent);
     
   }

   private void LateUpdate()
   {
      if (cam == null)
      {
         cam = Camera.main;
         return;
      }

      transform.rotation = cam.transform.rotation;
   }
}
