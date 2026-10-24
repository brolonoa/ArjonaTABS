using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class VampireBeam : MonoBehaviour
{
    [SerializeField] private Material beamMaterial;
    [SerializeField] private Color beamColor = new Color(1f, 0.25f, 0.2f, 0.9f);
    [SerializeField] private float startWidth = 0.18f;
    [SerializeField] private float endWidth = 0.08f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float originHeight = 1.3f;
    [SerializeField] private float targetHeight = 1f;

    private EnemyController controller;
    private LineRenderer line;
    private Coroutine beamCoroutine;

    private void Awake()
    {
        controller = GetComponentInParent<EnemyController>();

        GameObject beam = new GameObject("VampireBeam");
        beam.transform.SetParent(transform, false);

        line = beam.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.material = beamMaterial;
        line.startWidth = startWidth;
        line.endWidth = endWidth;
        line.numCapVertices = 4;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = false;
    }

    // Called by the animation event of the vampire attack, at the same time as VampireAttack.Hit
    public void VampireHit()
    {
        TeamMember target = controller != null ? controller.currentTarget : null;

        if (target == null || controller.teamMember == null || !controller.teamMember.IsEnemy(target))
        {
            return;
        }

        if (beamCoroutine != null)
        {
            StopCoroutine(beamCoroutine);
        }

        beamCoroutine = StartCoroutine(ShowBeam(target));
    }

    private IEnumerator ShowBeam(TeamMember target)
    {
        line.enabled = true;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (target == null)
            {
                break;
            }

            Vector3 from = transform.position + Vector3.up * originHeight;
            Vector3 to = target.Position + Vector3.up * targetHeight;

            line.SetPosition(0, from);
            line.SetPosition(1, to);

            Color color = beamColor;
            color.a *= 1 - t / duration;
            line.startColor = color;
            line.endColor = color;

            Debug.DrawLine(from, to, beamColor);
            yield return null;
        }

        line.enabled = false;
        beamCoroutine = null;
    }
}
