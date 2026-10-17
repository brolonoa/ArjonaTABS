using UnityEngine;

public class PotionEffects : MonoBehaviour
{
    [SerializeField] private GameObject throwEffect;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float impactLifeTime = 2f;

    private bool isQuitting;

    private void Awake()
    {
        throwEffect.SetActive(true);
        impactEffect.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (isQuitting || !gameObject.scene.isLoaded || impactEffect == null)
        {
            return;
        }

        GameObject impact = Instantiate(impactEffect, impactEffect.transform.position, Quaternion.identity);
        impact.transform.localScale = impactEffect.transform.lossyScale;

        foreach (Renderer meshRenderer in impact.GetComponentsInChildren<Renderer>(true))
        {
            if (!(meshRenderer is ParticleSystemRenderer))
            {
                meshRenderer.enabled = false;
            }
        }

        impact.SetActive(true);
        Destroy(impact, impactLifeTime);
    }
}
