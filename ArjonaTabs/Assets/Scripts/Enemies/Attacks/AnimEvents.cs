using UnityEngine;

public class AnimEvents : MonoBehaviour
{
   [SerializeField] private MeleeAttack _meleeAttack;
   [SerializeField] private RangeAttack _rangeAttack;
   [SerializeField] private VampireAttack _vampireAttack;
   public void EnableHurtBox()
   {
      _meleeAttack.EnableDamage();
   }
   public void DisableHurtBox()
   {
      _meleeAttack.DisableDamage();
   }

   public void InstantiateProjectile()
   {
      _rangeAttack.InstantiateProjectile();
   }

   public void VampireHit()
   {
      _vampireAttack.Hit();
   }
}
