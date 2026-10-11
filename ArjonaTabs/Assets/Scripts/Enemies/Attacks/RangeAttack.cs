 using UnityEngine;

 public class RangeAttack : EnemyAttack
 {
     //public GameObject projectilePrefab;
    
     [SerializeField] private PotionProjectile potionPrefab;
     [SerializeField] private Transform throwPoint;
      private Transform target;

     [SerializeField] private float flightTime = 1.2f;

     private TeamMember teamMember;
     

     private void Awake()
     {
         teamMember = GetComponentInParent<TeamMember>();
     }
     public override void StartAttack()
     {
         sm.anim.SetTrigger("attack");
         target = sm.currentTarget.gameObject.transform;
     }

     public void InstantiateProjectile()
     {
         if (target == null || potionPrefab == null || throwPoint == null)
             return;

         Vector3 aimPoint = target.position;
         
         potionPrefab.damage = sm.stats.damage;
         Collider col = target.GetComponentInChildren<Collider>();
         if (col != null)
             aimPoint = col.bounds.center;

         PotionProjectile potion = Instantiate(potionPrefab, throwPoint.position, throwPoint.rotation);
         potion.Launch(aimPoint, teamMember, flightTime);
     }
    
        
 }
