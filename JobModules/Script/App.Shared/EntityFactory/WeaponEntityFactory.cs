using App.Shared;
using App.Shared.GameModules.Weapon;
using Core;
using Core.EntityComponent;

namespace Assets.App.Shared.EntityFactory
{
    public static class WeaponEntityFactory
    {

        private static WeaponContext weaponContext;
        
        public static void SetContext(WeaponContext context)
        {
            weaponContext = context;
        }
        /// <summary>
        /// orient.configId为当前weaponId，Entitykey如果取得到，则返回WeaponEnity，取不到则重新生成Enitykey并创建；
        /// </summary>
        /// <param name="context"></param>
        /// <param name="playerKey"></param>
        /// <param name="orient"></param>
        /// <returns></returns>
        public static WeaponEntity GetOrCreateWeaponEntity(WeaponContext context, EntityKey playerKey, ref WeaponScanStruct orient)
        {
            WeaponEntity weaponEntity = context.GetEntityWithEntityKey(orient.WeaponKey);
            if (weaponEntity == null)
            {
                weaponEntity = context.CreateEntity();
                weaponEntity.AddWeaponBasicData();
               
                weaponEntity.AddWeaponRuntimeData();
                weaponEntity.AddWeaponScan();
                weaponEntity.AddEntityKey(new EntityKey(Contexts.sharedInstance.session.commonSession.EntityIdGenerator.GetNextEntityId(),
                    (short)EEntityType.Weapon));
            }
            orient.WeaponKey = weaponEntity.entityKey.Value;
            weaponEntity.weaponBasicData.SyncSelf(orient);
            weaponEntity.SetFlagHasOwnwer(playerKey);
            return weaponEntity;
        }
        public static WeaponEntity GetWeaponEntity(WeaponContext context, EntityKey weaponKey)
        {
            return context.GetEntityWithEntityKey(weaponKey);

        }
        //public static void RemoveWeaponEntity(PlayerEntity playerEntity, Contexts contexts,  bagIndex, EWeaponSlotType slotIndex)
        //{
        //    WeaponEntity weaponEntity;

        //    if (!playerEntity.TryGetWeapon(contexts, out weaponEntity))
        //    {
        //        return;
        //    }
        //    if (SharedConfig.IsServer)
        //    {
        //        weaponEntity.SetFlagNoOwner();
        //    }
        //    playerEntity.SetWeaponInSlot(slotIndex, 0);
        //}

    }
}