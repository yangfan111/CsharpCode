namespace App.Shared.GameModules.Attack
{
    public static class AttackUtil
    {
        public static int GeneraterUniqueHitId(PlayerEntity srcPlayer, int cmdSeq)
        {
            return (srcPlayer.entityKey.Value.EntityId << 16) + cmdSeq;
        }
    }
}