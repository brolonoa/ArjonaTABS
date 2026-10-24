using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
   [SerializeField] private EnemyHp enemyHp;
   [SerializeField] private Material flashMaterial;
   [SerializeField] private float flashDuration = 0.2f;

   private Renderer[] renderers;
   private Material[][] originalMaterials;
   private Material[][] flashMaterials;
   private Coroutine flashCoroutine;

   private void Start()
   {
      renderers = GetComponentsInChildren<Renderer>();
      originalMaterials = new Material[renderers.Length][];
      flashMaterials = new Material[renderers.Length][];

      for (int i = 0; i < renderers.Length; i++)
      {
         originalMaterials[i] = renderers[i].sharedMaterials;
         flashMaterials[i] = new Material[originalMaterials[i].Length];
         for (int j = 0; j < flashMaterials[i].Length; j++)
         {
            flashMaterials[i][j] = flashMaterial;
         }
      }
   }

   private void OnEnable()
   {
      if (enemyHp == null)
      {
         enemyHp = GetComponent<EnemyHp>();
      }

      if (enemyHp != null)
      {
         enemyHp.OnDamaged += Flash;
      }
   }

   private void OnDisable()
   {
      if (enemyHp != null)
      {
         enemyHp.OnDamaged -= Flash;
      }
   }

   private void Flash(float damage)
   {
      if (renderers == null)
      {
         return;
      }

      Flash();
   }

   private void Flash()
   {
      if (flashCoroutine != null)
      {
         StopCoroutine(flashCoroutine);
      }

      flashCoroutine = StartCoroutine(FlashRoutine());
   }

   private IEnumerator FlashRoutine()
   {
      SetMaterials(flashMaterials);
      yield return new WaitForSeconds(flashDuration);
      SetMaterials(originalMaterials);
      flashCoroutine = null;
   }

   private void SetMaterials(Material[][] materials)
   {
      for (int i = 0; i < renderers.Length; i++)
      {
         if (renderers[i] != null)
         {
            renderers[i].sharedMaterials = materials[i];
         }
      }
   }
}
