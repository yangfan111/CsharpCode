using Core.EntityComponent;

namespace App.Shared.WeaponLogic
{
    public static class WeaponEntityEx
    {
        public static void AddOwner(this WeaponEntity weaponEntity, EntityKey entityKey)
        {
            if (!weaponEntity.hasOwnerId)
            {
                weaponEntity.AddOwnerId(entityKey);
            }
            else
            {
                weaponEntity.ownerId.Value = entityKey;
            }
            weaponEntity.isFlagSyncSelf = true;
        }

        public static void RemoveOwner(this WeaponEntity weaponEntity)
        {
            weaponEntity.isFlagSyncSelf = false;
        }

        public static void Activate(this WeaponEntity weaponEntity, bool activate)
        {
            weaponEntity.isActive = activate;
        }
    }
}
