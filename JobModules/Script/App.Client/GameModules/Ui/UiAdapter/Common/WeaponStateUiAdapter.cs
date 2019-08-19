using App.Shared;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Components.Player;
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

                return 0;
            }
        }

        public int WeaponIdByIndex(int index)
        {
            var agent = Archive.GetWeaponAgent(Index2Slot(index));
            return agent.BaseComponent.ConfigId;
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
                    var greandeId = agent.BaseComponent.ConfigId;
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

            var configId = WeaponIdByIndex(index);

            if (configId < 1)
            {
                return 0;
            }

            var weaponCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(configId);
            if (null == weaponCfg)
            {
                return 0;
            }

            switch ((EWeaponType_Config) weaponCfg.Type)
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
                return _contexts.ui.uI.Player;
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
            return (int) (slot - EWeaponSlotType.PrimeWeapon) + 1;
        }

        private IPlayerWeaponSharedGetter _arhive;

        private IPlayerWeaponSharedGetter Archive
        {
            get
            {
                return Player.WeaponController();
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
            var _grenadeList1 = helper.GetOwnedIds();

            var realIndex = grenadeIndex - 1;
            if (realIndex < 0 || realIndex >= _grenadeList1.Count)
            {
                return 0;
            }

            return _grenadeList1[realIndex];
        }

        public int CurrentGrenadeIndex
        {
            get
            {
                var index = Archive.GrenadeHandler.GetHoldGrenadeIndex();
                index += 1;
                return index;
            }

        }

        public virtual List<int> SlotIndexList
        {
            get { return _slotIndexList; }
        }

        private List<int> _slotIndexList = new List<int> {1, 2, 3, 4, 5};

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

            return GrenadeIdByIndex(grenadeIndex) > 0;
        }

        private bool BioCloseShow
        {
            get
            {
                var modeId = _contexts.session.commonSession.RoomInfo.ModeId;
                return (GameRules.IsBio(modeId) && Player.hasGamePlay &&
                        Player.gamePlay.JobAttribute != (int) EJobAttribute.EJob_EveryMan);

            }

        }

        public override bool Enable
        {
            get { return base.Enable && !BioCloseShow; }

            set { base.Enable = value; }
        }
    }

}

        

    