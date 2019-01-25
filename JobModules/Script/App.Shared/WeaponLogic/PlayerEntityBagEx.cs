using App.Shared.Components.Bag;
using Core;
using Core.EntityComponent;
using Core.Utils;

namespace App.Shared.WeaponLogic
{
    public static class PlayerEntityBagEx
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityBagEx));

        //TODO 保证背包索引和槽位都合法的情况下，一定有武器
        public static bool HasWeapon(this PlayerEntity playerEntity, EWeaponSlotType slot)
        {
            return (slot > EWeaponSlotType.None
                && playerEntity.bagState.CurBag > (int)EWeaponBagIndex.None
                && playerEntity.bagState.CurBag < (int)EWeaponBagIndex.Length
                && slot < EWeaponSlotType.Length);
        }

        public static bool HasWeapon(this PlayerEntity playerEntity, Contexts contexts)
        {
            WeaponEntity weaponEntity;
            return playerEntity.TryGetWeapon(contexts, 
                (EWeaponBagIndex)playerEntity.bagState.CurBag, 
                (EWeaponSlotType)playerEntity.bagState.CurSlot, 
                out weaponEntity);
        }

        public static bool HasWeaponInSlot(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            WeaponEntity weaponEntity;
            return playerEntity.TryGetWeapon(contexts, 
                (EWeaponBagIndex)playerEntity.bagState.CurBag, 
                slot,
                out weaponEntity);
        }

        public static bool TryGetWeapon(this PlayerEntity playerEntity,
            Contexts contexts, out WeaponEntity weaponEntity)
        {
            return TryGetWeapon(playerEntity, 
                contexts, 
                (EWeaponBagIndex)playerEntity.bagState.CurBag, 
                (EWeaponSlotType)playerEntity.bagState.CurSlot,
                out weaponEntity);
        }

        public static bool TryGetWeapon(this PlayerEntity playerEntity,
            Contexts contexts, EWeaponSlotType slot, out WeaponEntity weaponEntity)
        {
            return TryGetWeapon(playerEntity, 
                contexts, 
                (EWeaponBagIndex)playerEntity.bagState.CurBag, 
                slot,
                out weaponEntity);
        }

        public static bool TryGetWeapon(this PlayerEntity playerEntity, 
            Contexts contexts, 
            EWeaponBagIndex bagIndex, 
            EWeaponSlotType slot, 
            out WeaponEntity weaponEntity)
        {
            if (slot <= EWeaponSlotType.None
                || bagIndex <= EWeaponBagIndex.None
                || bagIndex >= EWeaponBagIndex.Length
                || slot >= EWeaponSlotType.Length)
            {
                weaponEntity = new WeaponEntity();
                return false;
            }
            var entityId = GetWeaponInSlot(playerEntity, slot);
            if (!entityId.HasValue)
            {
                weaponEntity = new WeaponEntity();
                return false;
            }
            weaponEntity = contexts.weapon.GetEntityWithEntityKey(new EntityKey(entityId.Value, (short)EEntityType.Weapon));
            return null != weaponEntity;
        }

        public static int? GetWeaponInSlot(this PlayerEntity playerEntity, EWeaponSlotType slot)
        {
            var weaponBag = playerEntity.GetWeaponBagComponent((EWeaponBagIndex)playerEntity.bagState.CurBag);
            if(null != weaponBag)
            {
                switch(slot)
                {
                    case EWeaponSlotType.PrimeWeapon:
                        return weaponBag.PrimeWeapon;
                    case EWeaponSlotType.SecondaryWeapon:
                        return weaponBag.SecondaryWeapon;
                    case EWeaponSlotType.PistolWeapon:
                        return weaponBag.PistolWeapon;
                    case EWeaponSlotType.MeleeWeapon:
                        return weaponBag.MeleeWeapon;
                    case EWeaponSlotType.ThrowingWeapon:
                        return weaponBag.ThrowingWeapon;
                    case EWeaponSlotType.TacticWeapon:
                        return weaponBag.TacticWeapon;
                }
            }
            return null;
        }

        public static void SetWeaponInSlot(this PlayerEntity playerEntity, EWeaponBagIndex bagIndex, EWeaponSlotType slot, int weaponEntityId)
        {
            var weaponBag = playerEntity.GetWeaponBagComponent((EWeaponBagIndex)playerEntity.bagState.CurBag);
            if(null != weaponBag)
            {
                switch(slot)
                {
                    case EWeaponSlotType.PrimeWeapon:
                        weaponBag.PrimeWeapon = weaponEntityId;
                        break;
                    case EWeaponSlotType.SecondaryWeapon:
                        weaponBag.SecondaryWeapon = weaponEntityId;
                        break;
                    case EWeaponSlotType.PistolWeapon:
                        weaponBag.PistolWeapon = weaponEntityId;
                        break;
                    case EWeaponSlotType.MeleeWeapon:
                        weaponBag.MeleeWeapon = weaponEntityId;
                        break;
                    case EWeaponSlotType.ThrowingWeapon:
                        weaponBag.ThrowingWeapon = weaponEntityId;
                        break;
                    case EWeaponSlotType.TacticWeapon:
                        weaponBag.TacticWeapon = weaponEntityId;
                        break;
                }
            }
            else
            {
                Logger.ErrorFormat("no bag in index ", (EWeaponBagIndex)playerEntity.bagState.CurBag);
            }
        }

        public static void SetWeaponInSlot(this PlayerEntity playerEntity, EWeaponSlotType slot, int weaponEntityId)
        {
            playerEntity.SetWeaponInSlot((EWeaponBagIndex)playerEntity.bagState.CurBag, slot, weaponEntityId);
        }

        public static WeaponBagComponent GetWeaponBagComponent(this PlayerEntity playerEntity)
        {
            return GetWeaponBagComponent(playerEntity, (EWeaponBagIndex)playerEntity.bagState.CurBag);
        }

        public static WeaponBagComponent GetWeaponBagComponent(this PlayerEntity playerEntity, EWeaponBagIndex index)
        {
            switch(index)
            {
                case EWeaponBagIndex.First:
                    return playerEntity.firstWeaponBag;
                case EWeaponBagIndex.Second:
                    return playerEntity.secondaryWeaponBag;
                case EWeaponBagIndex.Third:
                    return playerEntity.thirdWeaponBag;
                case EWeaponBagIndex.Forth:
                    return playerEntity.forthWeaponBag;
                case EWeaponBagIndex.Fifth:
                    return playerEntity.fifthWeaponBag;
            }
            return null;
        }

        public static WeaponEntity AddWeaponEntity(this PlayerEntity playerEntity, Contexts contexts, WeaponInfo weaponInfo)
        {
            var weaponEntity = contexts.weapon.CreateEntity();
            weaponEntity.AddWeaponData();
            weaponInfo.ToWeaponComponent(weaponEntity.weaponData);
            weaponEntity.AddEntityKey(new EntityKey(contexts.session.commonSession.EntityIdGenerator.GetNextEntityId(), 
                (short)EEntityType.Weapon));
            weaponEntity.AddOwnerId(playerEntity.entityKey.Value);
            weaponEntity.isFlagSyncSelf = true;
            return weaponEntity;
        }

        public static void RemoveWeaponEntity(this PlayerEntity playerEntity, Contexts contexts, EWeaponBagIndex bagIndex, EWeaponSlotType slotIndex)
        {
            WeaponEntity weaponEntity;
            if(!playerEntity.TryGetWeapon(contexts, out weaponEntity))
            {
                return;
            }
            if(SharedConfig.IsServer)
            {
                weaponEntity.isFlagSyncSelf = false;
            }
            playerEntity.SetWeaponInSlot(slotIndex, 0);
        }
    }
}