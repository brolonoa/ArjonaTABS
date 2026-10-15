using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
   private static Material outlineMaterial;

   private TextMeshPro text;
   private Camera cam;
   private Color color;
   private float lifeTime;
   private float riseSpeed;
   private float timer;

   public static void Spawn(Vector3 position, string value, Color color, float fontSize, float lifeTime, float riseSpeed)
   {
      GameObject go = new GameObject("DamageText");
      go.transform.position = position;

      FloatingDamageText floating = go.AddComponent<FloatingDamageText>();
      floating.Setup(value, color, fontSize, lifeTime, riseSpeed);
   }

   private void Setup(string value, Color newColor, float fontSize, float newLifeTime, float newRiseSpeed)
   {
      text = gameObject.AddComponent<TextMeshPro>();
      text.text = value;
      text.fontSize = fontSize;
      text.fontStyle = FontStyles.Bold;
      text.alignment = TextAlignmentOptions.Center;
      text.textWrappingMode = TextWrappingModes.NoWrap;
      text.rectTransform.sizeDelta = new Vector2(4, 1);

      if (outlineMaterial == null)
      {
         outlineMaterial = new Material(text.fontSharedMaterial);
         outlineMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
         outlineMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.25f);
         outlineMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
         outlineMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.2f);
      }
      text.fontSharedMaterial = outlineMaterial;

      color = newColor;
      text.color = color;
      lifeTime = newLifeTime;
      riseSpeed = newRiseSpeed;
      cam = Camera.main;
      transform.localScale = Vector3.one * 1.4f;
   }

   private void Update()
   {
      timer += Time.deltaTime;
      float t = timer / lifeTime;

      if (t >= 1)
      {
         Destroy(gameObject);
         return;
      }

      transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
      transform.localScale = Vector3.one * Mathf.Lerp(1.4f, 1f, Mathf.Clamp01(t * 5));

      Color c = color;
      c.a = t < 0.5f ? 1 : 1 - (t - 0.5f) * 2;
      text.color = c;

      if (cam != null)
      {
         transform.rotation = cam.transform.rotation;
      }
   }
}
