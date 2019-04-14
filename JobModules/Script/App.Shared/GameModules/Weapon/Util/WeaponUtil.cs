using System;
using App.Shared;
using App.Shared.Components.Weapon;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.CharacterState;
using Core.Configuration;
using Core.EntityComponent;
using Core.Room;
using System.Collections.Generic;
using App.Shared.GameModules.Weapon;
using Core.Prediction.UserPrediction.Cmd;
using Sharpen;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="WeaponUtil" />
    /// </summary>
    public static class WeaponUtil
    {
        public static int EmptyHandId
        {
            get { return SingletonManager.Get<WeaponResourceConfigManager>().EmptyHandId; }
        }

    
        //public static readonly WeaponEntity EmptyWeapon = new WeaponEntity();
        //public readonly static WeaponRuntimeDataComponent EmptyRun = new WeaponRuntimeDataComponent();

        //public readonly static WeaponBasicDataComponent EmptyWeaponBase = new WeaponBasicDataComponent();
        public static EWeaponSlotType GetEWeaponSlotTypeById(int weaponId)
        {
            var configType =
                (EWeaponType_Config)SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId).Type;
            return configType.ToWeaponSlot();
        }


        public static bool VertifyWeaponConfigId(int weaponId, out WeaponResConfigItem config)
        {
            config = null;
            if (weaponId == 0)
                return false;
            config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
            return config != null;
        }

        //public static List<InterruptData> CreateInterruptData()
        //{
        //    var InterruptDatas = new List<InterruptData>((int) EInterruptWeaponType.Count);
        //    for (int i = 0; i < (int) EInterruptWeaponType.Count; i++)
        //    {
        //        InterruptDatas.Add(i,new InterruptData());
        //    }

        //    return InterruptDatas;
        //}


        public static List<WeaponBagContainer> CreateEmptyBagContainers()
        {
            var containerSet = new List<WeaponBagContainer>(GlobalConst.WeaponBagMaxCount);
            for (int i = 0; i < GlobalConst.WeaponBagMaxCount; i++)
            {
                containerSet.Add(new WeaponBagContainer());
            }

            return containerSet;
        }

        public static WeaponScanStruct CreateScan(int configId, System.Action<WeaponScanStruct> initFunc)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.ConfigId = configId;
            initFunc(val);
            return val;
        }
        public static InterruptData CreateInterrupt(EInterruptCmdType cmdType)
        {
            var data = new InterruptData();
            data.hasValue = true;
            data.cmdType = (byte)cmdType;
            return data;
        }
        public static WeaponScanStruct CreateScan(PlayerWeaponData weaponData)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.ConfigId  = weaponData.WeaponTplId;
            val.AvatarId  = weaponData.WeaponTplId;
            val.UpperRail = weaponData.UpperRail;
            val.LowerRail = weaponData.LowerRail;
            val.Magazine  = weaponData.Magazine;
            val.Muzzle    = weaponData.Muzzle;
            val.Stock     = weaponData.Stock;
            return val;
        }

        //public static WeaponScanStruct CreateScan(Components.SceneObject.WeaponObjectComponent weaponObject)
        //{
        //    WeaponScanStruct val = new WeaponScanStruct();
        //    val.Assign(weaponObject.ConfigId);
        //    val.AvatarId = weaponObject.WeaponAvatarId;
        //    val.UpperRail = weaponObject.UpperRail;
        //    val.LowerRail = weaponObject.LowerRail;
        //    val.Magazine = weaponObject.Magazine;
        //    val.Muzzle = weaponObject.Muzzle;
        //    val.Stock = weaponObject.Stock;
        //    return val;
        //}

        public static WeaponScanStruct CreateScan(int configId)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.ConfigId = configId;
            return val;
        }

        public static WeaponScanStruct CreateScan()
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.ConfigId = EmptyHandId;
            return val;
        }

        public static WeaponScanStruct CreateScan(WeaponEntity entity)
        {
            WeaponScanStruct val = new WeaponScanStruct();
            val.ConfigId = entity.weaponBasicData.ConfigId;
            return val;
        }

        public static bool VertifyEweaponSlotIndex(int index, bool ignoreNone = false)
        {
            return (index > 0 || ignoreNone) && index < (int) EWeaponSlotType.Length;
        }


        public static List<GrenadeCacheData> CreateEmptyGrenadeCacheArrs(List<int> ids)
        {
            var containerSet = new List<GrenadeCacheData>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                containerSet.Add(new GrenadeCacheData());
                containerSet[i].grenadeId = ids[i];
            }

            return containerSet;
        }

        public static List<int> ForeachFilterGreandeIds()
        {
            var configs          = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigs();
            var grenadeConfigIds = new List<int>();
            foreach (var config in configs)
            {
                switch ((EWeaponType_Config) config.Value.Type)
                {
                    case EWeaponType_Config.ThrowWeapon:
                        var subType = (EWeaponSubType) config.Value.SubType;
                        switch (subType)
                        {
                            case EWeaponSubType.BurnBomb:
                            case EWeaponSubType.FlashBomb:
                            case EWeaponSubType.FogBomb:
                            case EWeaponSubType.Grenade:
                                break;
                            default:
                                break;
                        }

                        grenadeConfigIds.Add(config.Value.Id);
                        break;
                }
            }

            return grenadeConfigIds;
        }


        public static bool IsC4p(int configId)
        {
            return SingletonManager.Get<WeaponResourceConfigManager>().IsC4(configId);
        }


        public static float GetHolsterParam(EWeaponSlotType slot)
        {
            return GetHolsterParam(slot == EWeaponSlotType.SecondaryWeapon);
        }

        public static float GetHolsterParam(bool val)
        {
            return val
                ? AnimatorParametersHash.Instance.HolsterFromLeftValue
                : AnimatorParametersHash.Instance.HolsterFromRightValue;
        }
        public static WeaponSystemImplStruct? FilterWeaponSystemCmd(IUserCmdOwner owner, IUserCmd cmd,
                                                                    WeaponSystemCmdSpecificFilter specificFilter)
        {
            if (!cmd.IsReload)
                return null;
            PlayerEntity player = (PlayerEntity) owner.OwnerEntity;
            if (player == null)
                return null;
            PlayerWeaponController controller = player.WeaponController();
            if (controller == null) 
                return null;
            if (!specificFilter( player,controller, cmd))
                return null;
//            if (!cmd.FilteredInput.IsInput(EPlayerInput.IsReload) && !player.playerMove.IsAutoRun)
//                return null;
//            if (!controller.HeldWeaponAgent.IsValid())
//                return null;
            WeaponSystemImplStruct systemImplStruct = new WeaponSystemImplStruct();
            systemImplStruct.playerEntity           = player;
            systemImplStruct.weaponController = controller;
            return systemImplStruct;
        }
    }
}