using WeaponConfigNs;

namespace Core.Attack
{
    public enum MeleeAttckType
    {
        LeftMeleeAttack,
        RightMeleeAttack,
    }

    public struct MeleeAttackInfo
    {
        public MeleeAttckType AttackType;
    }
}
