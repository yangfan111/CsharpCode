using App.Shared.Components.Bag;
using App.Shared.Components.Weapon;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.WeaponLogic
{
    public static class PlayerEntityWeaponLogicEx
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityWeaponLogicEx));

        public static ExpandWeaponLogicConfig GetWeaponDataConfig(this PlayerEntity playerEntity, 
            Contexts contexts,
            IPlayerWeaponConfigManager playerWeaponConfigManager)
        {
            var weaponData = playerEntity.GetCurrentWeaponData(contexts);
            return playerWeaponConfigManager.GetWeaponLogicConfig(weaponData.WeaponId, new WeaponPartsStruct
            {
                LowerRail = weaponData.LowerRail,
                UpperRail = weaponData.UpperRail,
                Magazine = weaponData.Magazine,
                Stock = weaponData.Stock,
                Muzzle = weaponData.Muzzle,
            });
        }

        public static ExpandWeaponLogicConfig GetWeaponDataConfig(this PlayerEntity playerEntity, 
            Contexts contexts,
            int weaponId,
            WeaponPartsStruct weaponPartsStruct,
            IPlayerWeaponConfigManager playerWeaponConfigManager)
        {
            return playerWeaponConfigManager.GetWeaponLogicConfig(weaponId, weaponPartsStruct);
        }

        public static void PlayWeaponSound(this PlayerEntity playerEntity, EWeaponSoundType sound)
        {
            if(playerEntity.hasWeaponSound)
            {
                playerEntity.weaponSound.PlayList.Add(sound);
            }
        }

        /// <summary>
        /// 如果没有武器会拿到空手的值，而不是空值
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="contexts"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static WeaponBasicInfoComponent GetWeaponData(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            var weaponEntity = playerEntity.GetWeaponEntityByHandDefault(contexts, slot);
            if(null != weaponEntity)
            {
                return weaponEntity.weaponBasicInfo;
            }
            Logger.Error("weapon entity is null and no hand default provided");
            return null;
        }

        public static WeaponRuntimeInfoComponent GetWeaponRunTimeInfo(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
             var weaponEntity = playerEntity.GetWeaponEntityByHandDefault(contexts, slot);
            if(null != weaponEntity)
            {
                return weaponEntity.weaponRuntimeInfo;
            }
            Logger.Error("weapon entity is null and no hand default provided");
            return null;
        }

        public static BagStateComponent GetBagState(this PlayerEntity playerEntity)
        {
            return playerEntity.bagState;
        }

        public static WeaponBasicInfoComponent GetCurrentWeaponData(this PlayerEntity playerEntity, Contexts contexts)
        {
            return GetWeaponData(playerEntity, contexts, (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        public static WeaponRuntimeInfoComponent GetWeaponRunTimeInfo(this PlayerEntity playerEntity, Contexts contexts)
        {
            return GetWeaponRunTimeInfo(playerEntity, contexts, (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        public static WeaponScanInfo GetWeaponInfo(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
            {
                return GetEmptyHandWeapon(contexts, playerEntity);
            }
            var weaponEntityId = playerEntity.GetWeaponInSlot(slot);
            if(!weaponEntityId.HasValue)
            {
                return GetEmptyHandWeapon(contexts, playerEntity);
            }
            var weaponContext = contexts.weapon;
            var weaponKey = new EntityKey(weaponEntityId.Value, (short)EEntityType.Weapon);
            var weaponEntity = weaponContext.GetEntityWithEntityKey(weaponKey);
            if(null == weaponEntity)
            {
                return GetEmptyHandWeapon(contexts, playerEntity);
            }
            return weaponEntity.ToWeaponInfo();
        }

        private static WeaponScanInfo GetEmptyHandWeapon(Contexts contexts, PlayerEntity playerEntity)
        {
            return playerEntity.GetEmptyHandEntity(contexts).ToWeaponInfo();
        }

        private static WeaponEntity GetEmptyHandEntity ( this PlayerEntity playerEntity, Contexts contexts)
        {
            return contexts.weapon.GetEntityWithEntityKey(new EntityKey(playerEntity.emptyHand.EntityId, (short)EEntityType.Weapon));
        }

        public static bool IsEmptyHand(this PlayerEntity playerEntity, Contexts contexts)
        {
            return playerEntity.GetController<PlayerWeaponController>().CurrSlotType == EWeaponSlotType.None 
                || playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponId(contexts) < 1;
        }

        public static WeaponEntity GetWeaponEntity(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            var weaponContext = contexts.weapon;
            if(slot == EWeaponSlotType.None)
            {
                return null;
            }
            var weaponKey = playerEntity.GetWeaponInSlot(slot);
            if(!weaponKey.HasValue)
            {
                return null;
            }
            return weaponContext.GetEntityWithEntityKey(new EntityKey(weaponKey.Value, (short)EEntityType.Weapon));
        }

        public static WeaponEntity GetWeaponEntityByHandDefault(this PlayerEntity playerEntity, Contexts contexts)
        {
            return playerEntity.GetWeaponEntityByHandDefault(contexts, (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        public static WeaponEntity GetWeaponEntityByHandDefault(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            var weaponEntity = playerEntity.GetWeaponEntity(contexts, slot);
            if(null != weaponEntity)
            {
                return weaponEntity;
            }
            return playerEntity.GetEmptyHandEntity(contexts);
        }

        public static WeaponScanInfo GetCurrentWeaponInfo(this PlayerEntity playerEntity, Contexts contexts)
        {
            var slot = (EWeaponSlotType)playerEntity.bagState.CurSlot;
            return GetWeaponInfo(playerEntity, contexts, slot);
        }

        public static ExpandWeaponLogicConfig GetWeaponConfig(this PlayerEntity playerEntity, Contexts contexts)
        {
            return GetWeaponConfig(playerEntity, contexts, 
                (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        public static int GetMagazineCapacity(this PlayerEntity playerEntity, Contexts contexts)
        {
            var config = playerEntity.GetWeaponConfig(contexts).CommonFireCfg;
            if (null == config)
            {
                return 0;
            }
            return config.MagazineCapacity; 
        } 

        public static float GetBreathFactor(this PlayerEntity playerEntity, Contexts contexts) 
        {
            var config = playerEntity.GetWeaponConfig(contexts).DefaultFireLogicCfg;
            if(null == config)
            {
                return 1;
            }
            return config.BreathFactor;
        }

        public static float GetReloadSpeed(this PlayerEntity playerEntity, Contexts contexts)
        {
            var config = playerEntity.GetWeaponConfig(contexts).DefaultFireLogicCfg;
            if(null == config)
            {
                return 1;
            }
            return config.ReloadSpeed;
        }

        public static float GetDefaultSpeed(this PlayerEntity palyerEntity, Contexts contexts)
        {
            var emptyHand = SingletonManager.Get<WeaponConfigManager>().EmptyHandId.Value;
            var config = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(emptyHand);
            return config.WeaponLogic.MaxSpeed;
        }

        public static float GetBaseSpeed(this PlayerEntity playerEntity, Contexts contexts)
        {
            return playerEntity.GetWeaponConfig(contexts).GetSpeed();
        }

        public static float GetBaseFov(this PlayerEntity playerEntity, Contexts contexts)
        {
            var config = playerEntity.GetWeaponConfig(contexts).DefaultFireLogicCfg;
            if(null == config)
            {
                //TODO 配置
                return 90;
            }
            return config.Fov;
        }

        public static float GetFov(this PlayerEntity playerEntity, Contexts contexts)
        {
            if (playerEntity.IsFovModified(contexts))
            {
                return GetBaseFov(playerEntity, contexts);
            }
            else
            {
                if (playerEntity.oxygenEnergyInterface.Oxygen.InShiftState)
                {
                    WeaponEntity weaponEntity;
                    if (!playerEntity.TryGetWeapon(contexts, out weaponEntity))
                    {
                        return GetBaseFov(playerEntity, contexts);
                    }
                    var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponEntity.weaponBasicInfo.WeaponId);
                    if (null != weaponCfg)
                    {
                        return weaponCfg.ShiftFov;
                    }
                }
                return GetBaseFov(playerEntity, contexts);
            }
        }

        public static float GetFocusSpeed(this PlayerEntity playerEntity, Contexts contexts)
        {
            var config = playerEntity.GetWeaponConfig(contexts).DefaultFireLogicCfg;
            if(null == config)
            {
                return 1;
            }
            return config.FocusSpeed;
        }

        public static bool IsFovModified(this PlayerEntity playerEntity, Contexts contexts)
        {
            var data = playerEntity.GetCurrentWeaponData(contexts);
            if(null == data)
            {
                return false;
            }
            var config = playerEntity.GetWeaponConfig(contexts);
            if(null == config)
            {
                return false;
            }
            var srcConfig = SingletonManager.Get<WeaponDataConfigManager>().GetFireLogicConfig(data.WeaponId);
            if(null == srcConfig)
            {
                return false;
            }
            return srcConfig.Fov != config.GetGunSightFov();
        }

        public static bool IsCanGunSight(this PlayerEntity playerEntity, Contexts contexts)
        {
            var config = playerEntity.GetWeaponConfig(contexts);
            return null != config && null != config.DefaultFireLogicCfg;
        }

        public static ExpandWeaponLogicConfig GetWeaponConfig(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot)
        {
            return playerEntity.GetWeaponDataConfig(contexts, contexts.session.commonSession.PlayerWeaponConfigManager);
        }

        public static void SetWeaponData(this PlayerEntity playerEntity, Contexts contexts, EWeaponSlotType slot, int weaponEntityId)
        {
        }

        public static void RefreshPlayerWeaponLogic(this PlayerEntity player, Contexts contexts, int? weaponId)
        {
            if (!weaponId.HasValue)
            {
                return;
            }

            // 更新枪械时，后坐力重置 
            player.orientation.PunchPitch = 0;
            player.orientation.PunchYaw = 0;
            player.orientation.WeaponPunchPitch = 0;
            player.orientation.WeaponPunchYaw = 0;
            //TODO 更新武器状态
        }

        public static bool IsAlwaysEmptyReload(this PlayerEntity playerEntity, Contexts contexts)
        {
            var weaponData = playerEntity.GetCurrentWeaponData(contexts);
            if(null == weaponData)
            {
                return false;
            }
            return SingletonManager.Get<WeaponConfigManager>().IsSpecialType(weaponData.WeaponId, ESpecialWeaponType.ReloadEmptyAlways);
        }
    }
}
