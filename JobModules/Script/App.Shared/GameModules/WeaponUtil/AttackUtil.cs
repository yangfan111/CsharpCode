using WeaponConfigNs;

namespace App.Shared
{
    public static class AttackUtil
    {
        public static int GeneraterUniqueHitId(PlayerEntity srcPlayer, int cmdSeq)
        {
            return (srcPlayer.entityKey.Value.EntityId << 16) + cmdSeq;
        }

        public static float GetThrowingInitSpeed(this ThrowingConfig throwingConfig,bool isNear)
        {
            return isNear ? throwingConfig.NearInitSpeed : throwingConfig.FarInitSpeed;
        }
        
    }
}