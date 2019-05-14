using App.Shared;
using App.Shared.Components;
using App.Shared.Components.SceneObject;
using App.Shared.GameModules.Weapon;
using Core;
using Core.EntityComponent;
using Core.Utils;

namespace Assets.App.Shared.EntityFactory
{
    public static class WeaponEntityFactory
    {

        public static WeaponContext WeaponContxt { private get; set; }

        public static IEntityIdGenerator EntityIdGenerator { private get; set; }
 
      
        public static WeaponEntity CreateEntity(WeaponScanStruct? orient)
        {
            var weaponEntity = WeaponContxt.CreateEntity();
            weaponEntity.AddWeaponBasicData();
            weaponEntity.AddWeaponRuntimeData(true);
            weaponEntity.AddWeaponScan();
            weaponEntity.AddEntityKey(new EntityKey(EntityIdGenerator.GetNextEntityId(),
                (short)EEntityType.Weapon));
            if(orient.HasValue)
                weaponEntity.weaponBasicData.SyncSelf(orient.Value);
            return weaponEntity;
            //  weaponEntity.weaponBasicData.SyncSelf(orient);
        }
        public static WeaponEntity GetWeaponEntity( EntityKey weaponKey)
        {
            return WeaponContxt.GetEntityWithEntityKey(weaponKey);
        }
        public static void RemoveWeaponEntity(WeaponEntity entity)
        {
            AssertUtility.Assert(SharedConfig.IsServer, "only Server remove weapon directly");
            WeaponEntityExt.SetDestroy(entity);
        }


    }
}