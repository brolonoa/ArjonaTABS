using System.Collections.Generic;
using UnityEngine;

public class PotionProjectile : MonoBehaviour
{
    public float damage ;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask targetLayers = ~0;

    private Rigidbody rb;
    private TeamMember owner;
    private bool hasExploded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 targetPosition, TeamMember thrower, float flightTime)
    {
        owner = thrower;

        Vector3 displacement = targetPosition - transform.position;

        Vector3 launchVelocity =
            (displacement - 0.5f * Physics.gravity *
            flightTime * flightTime) / flightTime;

        rb.linearVelocity = launchVelocity;
        Debug.Log($"Origen: {transform.position} | Destino: {targetPosition} | Velocidad: {launchVelocity}");
        Debug.DrawLine(transform.position, targetPosition, Color.red, 3f);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded)
            return;

        TeamMember target = collision.collider.GetComponentInParent<TeamMember>();

        
        if (target != null)
        {
            if (target.CurrentTeam == owner.CurrentTeam)
                return;
        }
        else
        {
            
            if (!collision.gameObject.CompareTag("Ground"))
                return;
        }

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayers);

        HashSet<TeamMember> hitTargets = new HashSet<TeamMember>();

        foreach (Collider hitCollider in hitColliders)
        {
            TeamMember target =
                hitCollider.GetComponentInParent<TeamMember>();

            if (target == null || target == owner)
                continue;

            if (target.CurrentTeam == owner.CurrentTeam)
                continue;

            if (!hitTargets.Add(target))
                continue;

            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (rb == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }
}
