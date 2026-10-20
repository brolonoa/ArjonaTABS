using System.Collections.Generic;
using UnityEngine;

public class PotionProjectile : MonoBehaviour
{
    public float damage ;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask targetLayers = ~0;

    private Rigidbody rb;
    private TeamMember owner;
    private TeamMember.Team ownerTeam;
    private bool hasExploded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 targetPosition, TeamMember thrower, float flightTime)
    {
        owner = thrower;
        ownerTeam = thrower.CurrentTeam;

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
            if (target.CurrentTeam == ownerTeam)
                return;
        }
        else
        {

            if (!collision.gameObject.CompareTag("Ground"))
                return;
        }

        Explode(target);
    }

    // Single target: the enemy hit directly, or else the closest enemy to the impact inside explosionRadius
    private void Explode(TeamMember directHit)
    {
        hasExploded = true;

        TeamMember victim = directHit != null ? directHit : FindClosestEnemy();

        if (victim != null)
        {
            victim.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private TeamMember FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayers);

        TeamMember closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hitCollider in hitColliders)
        {
            TeamMember target = hitCollider.GetComponentInParent<TeamMember>();

            if (target == null || target == owner)
                continue;

            if (target.CurrentTeam == ownerTeam)
                continue;

            float distance = (target.Position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    private void OnDrawGizmos()
    {
        if (rb == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }
}
