using Core;
using Core.Utils;
using Utils.AssetManager;
using Utils.Configuration;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using System.Collections.Generic;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;
using App.Shared;
using Core.Configuration;
using App.Shared.WeaponLogic;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public class WeaponStateUiAdapter : UIAdapter, IWeaponStateUiAdapter
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponStateUiAdapter));

        private Contexts _contexts;
        private List<int>_grenadeList;
        public WeaponStateUiAdapter(Contexts contexts)
        {
            _contexts = contexts;

        }
      
        public int HoldWeaponSlotIndex
        {
            get
            {
                if(null != Archive)
                {
                    return Slot2Index(Archive.CurrSlotType);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int WeaponIdByIndex(int index)
        {
            if (Player.HasWeaponInSlot(_contexts, Index2Slot(index)))
            {
                var weapon = GetWeaponInfoByIndex(index);
                return weapon.Id;
            }
            return 0;
        }

        public AssetInfo GetAssetInfoById(int weaponId)
        {
            if (weaponId < 1)
            {
                return new AssetInfo();
            }
            var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            if (null == weaponCfg)
            {
                return new AssetInfo();
            }
            var icon = SingletonManager.Get<WeaponAvatarConfigManager>().GetKillIcon(weaponCfg.AvatorId);
            return icon;
        }


        public int WeaponAtkModeByIndex(int index)
        {
            if(null != Archive)
            {
                return Archive.GetSlotFireMode(_contexts, Index2Slot(index));
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
            if(slot == EWeaponSlotType.ThrowingWeapon || slot == EWeaponSlotType.TacticWeapon)
            {
                var player = Player;
                if (null != player)
                {
                    if (!player.HasWeaponInSlot(_contexts, EWeaponSlotType.ThrowingWeapon))
                    {
                        return 0;
                    }
                    var curGrenade = player.GetController<PlayerWeaponController>().GetSlotWeaponId(_contexts, EWeaponSlotType.ThrowingWeapon).Value;
                    var helper = player.GetController<PlayerWeaponController>().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
                    return helper.ShowCount(curGrenade);
                }
            }
            return Archive.GetSlotWeaponInfo(_contexts, Index2Slot(index)).Bullet;
        }

        public int WeaponReservedBulletByIndex(int index)
        {
            if (null == Archive)
            {
                return 0;
            }
            var weapon = Archive.GetSlotWeaponInfo(_contexts, Index2Slot(index));
            if(weapon.Id < 1)
            {
                return 0;
            }
            return Archive.GetReservedBullet(Index2Slot(index));
        }

        public bool HasWeaponByIndex(int index)
        {
            if(null == Archive)
            {
                return false;
            }

            var weapon = Archive.GetSlotWeaponInfo(_contexts, Index2Slot(index));
            return weapon.Id > 0;
        }

        public int WeaponTypeByIndex(int index)
        {
            if(!Player.HasWeaponInSlot(_contexts, Index2Slot(index)))
            {
                return 0;
            }
            var weapon = GetWeaponInfoByIndex(index);
            if(weapon.Id < 1)
            {
                return 0;
            }
            var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weapon.Id);
            if(null == weaponCfg)
            {
                return 0;
            }
            switch ((EWeaponType)weaponCfg.Type)
            {
                case EWeaponType.PrimeWeapon:
                case EWeaponType.SubWeapon:
                    return 1;
                case EWeaponType.ThrowWeapon:
                    return 2;
                case EWeaponType.MeleeWeapon:
                    return 3;
                default:
                    return 0;
            }
        }

        public int WeaponAtkModeNumberByIndex(int index)
        {
            if(null == Archive)
            {
                return 1;
            }
            if(Player.HasWeaponInSlot(_contexts, Index2Slot(index)))
            {
                var id = GetWeaponInfoByIndex(index).Id;
                return SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(id);
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

        PlayerEntity _player;

        public PlayerEntity Player
        {
            get
            {
                if (null == _player)
                {
                    var newPlayer = _contexts.player.flagSelfEntity;
                    _player = newPlayer;
                }
                return _player;
            }
            set { _player = value; }
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

        public WeaponInfo GetWeaponInfoByIndex(int index)
        {
            if(null != Archive)
            {
                return Archive.GetSlotWeaponInfo(_contexts, Index2Slot(index));
            }

            return new WeaponInfo();
        }

        private ISharedPlayerWeaponComponentGetter _arhive;

        private ISharedPlayerWeaponComponentGetter Archive
        {
            get
            {
                var player = Player;
                if (null == player)
                {
                    return null;
                }

                if (null == _arhive)
                    _arhive = player.GetController<PlayerWeaponController>();
                //if (!player.hasWeaponComponentAgent)
                //    return null;



                return _arhive; // as PlayerWeaponComponentAgent;
            }
            //set { _arhive = value; }
        }

        public int GrenadeIdByIndex(int grenadeIndex)
        {
            var player = Player;
            if (null == player)
            {
                return 0;
            }
            var helper = player.GetController<PlayerWeaponController>().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
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
                var player = Player;
                if(null == player)
                {
                    return 1;
                }
                var weaponId = player.GetController<PlayerWeaponController>().CurrSlotWeaponId(_contexts);
                if(weaponId < 1)
                {
                    return 1;
                }
                var helper = _player.GetController<PlayerWeaponController>().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
                _grenadeList = helper.GetOwnedIds();
                for(int i = 0; i < _grenadeList.Count; i++)
                {
                    if(_grenadeList[i] == weaponId)
                    {
                        return i + 1;
                    }
                }
                return 1;
            }
	    }

        public virtual List<int> SlotIndexList
        {
            get { return new List<int> {1, 2, 3, 4, 5}; }
        }

        public bool HasGrenadByIndex(int grenadeIndex)
		{
            var player = Player;
            if(null == player)
            {
                return false;
            }
            if(player.GetController<PlayerWeaponController>().CurrSlotType != EWeaponSlotType.ThrowingWeapon)
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