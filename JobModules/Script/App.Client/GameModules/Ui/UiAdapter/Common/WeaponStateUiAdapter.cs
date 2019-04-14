using App.Shared;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    /// <summary>
    /// Defines the <see cref="WeaponStateUiAdapter" />
    /// </summary>
    public class WeaponStateUiAdapter : UIAdapter, IWeaponStateUiAdapter
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponStateUiAdapter));

        private Contexts _contexts;

        private List<int> _grenadeList;

        public WeaponStateUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public int HoldWeaponSlotIndex
        {
            get
            {
                if (null != Archive)
                {
                    return Slot2Index(Archive.HeldSlotType);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int WeaponIdByIndex(int index)
        {
            if (!Archive.IsWeaponSlotEmpty(Index2Slot(index)))
            {
                var weapon = GetWeaponInfoByIndex(index);
                return weapon.ConfigId;
            }
            return 0;
        }

        public AssetInfo GetAssetInfoById(int weaponId)
        {
            if (weaponId < 1)
            {
                return new AssetInfo();
            }
            var weaponCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
            if (null == weaponCfg)
            {
                return new AssetInfo();
            }
            var icon = SingletonManager.Get<WeaponAvatarConfigManager>().GetKillIcon(weaponCfg.AvatorId);
            return icon;
        }

        public int WeaponAtkModeByIndex(int index)
        {
            if (null != Archive)
            {
                return Archive.GetWeaponAgent(Index2Slot(index)).BaseComponent.RealFireModel;
            }
            return 0;
        }

        public int WeaponBulletCountByIndex(int index)
        {
            if (null == Archive)
            {
                return 0;
            }
            var slot = Index2Slot(index);
            var realIndex = index - 1;
            if (slot == EWeaponSlotType.ThrowingWeapon || slot == EWeaponSlotType.TacticWeapon)
            {
                var player = Player;
                if (null != player)
                {
                    if (Archive.IsWeaponSlotEmpty(EWeaponSlotType.ThrowingWeapon))
                    {
                        return 0;
                    }

                    var agent = Archive.GetWeaponAgent(EWeaponSlotType.ThrowingWeapon);
                    var greandeId = Archive.GetWeaponAgent(EWeaponSlotType.ThrowingWeapon).ConfigId;
                    return Archive.GrenadeHandler.ShowCount(greandeId);
                }
            }
            return Archive.GetWeaponAgent(Index2Slot(index)).BaseComponent.Bullet;
        }

        public int WeaponReservedBulletByIndex(int index)
        {
            if (null == Archive || Archive.IsWeaponSlotEmpty(Index2Slot(index)))
            {
                return 0;
            }
            return Archive.GetReservedBullet(Index2Slot(index));
        }

        public bool HasWeaponByIndex(int index)
        {
            if (null == Archive)
            {
                return false;
            }
            return !Archive.IsWeaponSlotEmpty(Index2Slot(index));
        }

        public int WeaponTypeByIndex(int index)
        {
            if (Archive.IsWeaponSlotEmpty(Index2Slot(index)))
            {
                return 0;
            }
            var weapon = GetWeaponInfoByIndex(index);
            if (weapon.ConfigId < 1)
            {
                return 0;
            }
            var weaponCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weapon.ConfigId);
            if (null == weaponCfg)
            {
                return 0;
            }
            switch ((EWeaponType_Config)weaponCfg.Type)
            {
                case EWeaponType_Config.PrimeWeapon:
                case EWeaponType_Config.SubWeapon:
                    return 1;
                case EWeaponType_Config.ThrowWeapon:
                    return 2;
                case EWeaponType_Config.MeleeWeapon:
                    return 3;
                default:
                    return 0;
            }
        }

        public int WeaponAtkModeNumberByIndex(int index)
        {
            if (null != Archive)
            {
                return Archive.GetWeaponAgent(Index2Slot(index)).FireModeCount;
            }
            return 1;
        }

        public bool IsSwitchWeapon
        {
            get
            {
                var player = Player;
                if (null == player)
                {
                    return false;
                }
                var action = player.stateInterface.State.GetActionState();
                return action == XmlConfig.ActionInConfig.SwitchWeapon;

            }
        }


        public PlayerEntity Player
        {
            get
            {
                return _contexts.player.flagSelfEntity;
            }
        }

        public override bool IsReady()
        {
            return null != Player;
        }

        protected virtual EWeaponSlotType Index2Slot(int index)
        {
            return EWeaponSlotType.PrimeWeapon + (index - 1);
        }

        protected virtual int Slot2Index(EWeaponSlotType slot)
        {
            return (int)(slot - EWeaponSlotType.PrimeWeapon) + 1;
        }

        public WeaponScanStruct GetWeaponInfoByIndex(int index)
        {
            var baseScan = Archive.GetWeaponAgent(Index2Slot(index)).ComponentScan;
            return baseScan;
        }

        private IPlayerWeaponSharedGetter _arhive;

        private IPlayerWeaponSharedGetter Archive
        {
            get
            {
                if (_arhive == null)
                    _arhive = Player.WeaponController();
                return _arhive;
            }
        }

        public int GrenadeIdByIndex(int grenadeIndex)
        {
            var player = Player;
            if (null == player)
            {
                return 0;
            }
            var helper = Archive.GrenadeHandler;
            var _grenadeList = helper.GetOwnedIds();

            var realIndex = grenadeIndex - 1;
            if (realIndex < 0 || realIndex >= _grenadeList.Count)
            {
                return 0;
            }
        
            return _grenadeList[realIndex];
        }

        public int CurrentGrenadeIndex
        {
            get
            {
                int index = Archive.GrenadeHandler.GetHoldGrenadeIndex();
                if (index > 0) index += 1;
                return Math.Max(index, 1);
            }
            //    var player = Player;
            //    if (null == player)
            //    {
            //        return 1;
            //    }
            //    var weaponId = Archive.HeldWeaponAgent.ConfigId;
            //    if (weaponId < 1)
            //    {
            //        return 1;
            //    }
            //    int index = Archive.GrenadeHelper.GetOwnedIds().IndexOf(Archive.GrenadeHelper.LastGrenadeId);
            //    var helper = Archive.GrenadeHelper;
            //    _grenadeList = helper.GetOwnedIds();
            //    for (int i = 0; i < _grenadeList.Count; i++)
            //    {
            //        if (_grenadeList[i] == weaponId)
            //        {
            //            return i + 1;
            //        }
            //    }
            //    return 1;
            //}
        }

        public virtual List<int> SlotIndexList
        {
            get { return new List<int> { 1, 2, 3, 4, 5 }; }
        }

        public bool HasGrenadByIndex(int grenadeIndex)
        {
            var player = Player;
            if (null == player)
            {
                return false;
            }
            if (Archive.HeldSlotType != EWeaponSlotType.ThrowingWeapon)
            {
                return false;
            }
            else
            {
                return GrenadeIdByIndex(grenadeIndex) > 0;
            }
        }
    }
}
