using Entitas;
using WeaponConfigNs;

namespace App.Shared.Components.Player
{
    [Player]
    public class AttackDamageComponent : IComponent
    {
        public EBodyPart HitPart;
        public float Damage;

        public float GetAndResetDamage()
        {
            var damage = Damage;
            Damage = 0;
            return damage;
        }

        public EBodyPart GetAndResetHitPart()
        {
            var part = HitPart;
            HitPart = 0;
            return part;
        }
    }
}