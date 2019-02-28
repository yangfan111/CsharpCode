using App.Shared.Components.Ui.UiAdapter;
using Assets.Utils.Configuration;
using Core.Room;
using Core.Utils;
using System.Collections.Generic;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;
using InterruptReason = App.Shared.Components.Player.PlayerInterruptStateComponent.InterruptReason;


namespace App.Client.GameModules.Ui.UiAdapter
{

    public class WeaponBagItemInfo : IWeaponBagItemInfo
    {
        private readonly NewWeaponConfigItem _weaponConfig;
        private readonly WeaponAvatarConfigItem _weaponAvatarConfig;
        private readonly PlayerWeaponData _playerWeaponData;
        private readonly AssetInfo _magazineAsset; 
        private readonly AssetInfo _muzzleAsset; 
        private readonly AssetInfo _stockAsset; 
        private readonly AssetInfo _upperAsset; 
        private readonly AssetInfo _lowerAsset;

        public WeaponBagItemInfo(PlayerWeaponData playerWeaponData, NewWeaponConfigItem newWeaponConfig, WeaponAvatarConfigItem weaponAvatarConfigItem)
        {
            _playerWeaponData = playerWeaponData;
            _weaponConfig = newWeaponConfig;
            _weaponAvatarConfig = weaponAvatarConfigItem;
            if (_playerWeaponData.Muzzle > 0)
            {
                _muzzleAsset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_playerWeaponData.Muzzle);
            }
            if (_playerWeaponData.Magazine > 0)
            {
                _magazineAsset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_playerWeaponData.Magazine);
            }
            if (_playerWeaponData.Stock > 0)
            {
                _magazineAsset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_playerWeaponData.Stock);
            }
            if (_playerWeaponData.UpperRail > 0)
            {
                _magazineAsset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_playerWeaponData.UpperRail);
            }
            if (_playerWeaponData.LowerRail > 0)
            {
                _magazineAsset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(_playerWeaponData.LowerRail);
            }
        }

        public string Name
        {
            get
            {
                return _weaponConfig.Name;
            }
        }

        public AssetInfo WeaponIconAssetInfo
        {
            get
            {
                return new AssetInfo(_weaponAvatarConfig.IconBundle, _weaponAvatarConfig.Icon);
            }
        }

        public string WeaponName
        {
            get
            {
                return _weaponAvatarConfig.Name;
            }
        }


        public AssetInfo GetWeaponPartInfoByWeaponPartType(EWeaponPartType weaponPartType)
        {
            switch(weaponPartType)
            {
                case EWeaponPartType.LowerRail:
                    return _lowerAsset;
                case EWeaponPartType.UpperRail:
                    return _upperAsset;
                case EWeaponPartType.Stock:
                    return _stockAsset;
                case EWeaponPartType.Magazine:
                    return _magazineAsset;
                case EWeaponPartType.Muzzle:
                    return _muzzleAsset;
            }
            return new AssetInfo();
        }

    }

    public class WeaponBagUiAdapter : UIAdapter, IWeaponBagUiAdapter
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagUiAdapter));
        private Contexts _contexts;
        public PlayerEntity Player { get; set; }
        WeaponBagInfo[] _bagArray;
        WeaponBagInfo[] BagArray
        {
            get
            {
                if(null == _bagArray)
                {
                    var playerEntity = _contexts.player.flagSelfEntity;
                    if(null == playerEntity)
                    {
                        return null;
                    }
                    if(!playerEntity.hasPlayerInfo)
                    {
                        return null;
                    }
                    _bagArray = new WeaponBagInfo[6];
                    var bags = playerEntity.playerInfo.WeaponBags;
                    for(int i = 0; i < bags.Length; i++)
                    {
                        var bagInfo = bags[i];
                        if (null == bagInfo)
                        {
                            _bagArray[i] = null;
                        }
                        else
                        {
                            _bagArray[i] = new WeaponBagInfo(bags[i], playerEntity);
                        }
                    }
                }
                return _bagArray;
            }
        }

        public WeaponBagUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public override bool IsReady()
        {
            return Player != null;
        }

        public override bool Enable
        {
            get { return _enable && IsBagSwitchEnabled(); }

            set {
                if (!IsBagSwitchEnabled())
                {
                    _enable = false;
                    if(IsReady())SetGunSightFlag(false);
                }
                else
                {
                    if (value)
                    {
                        _contexts.session.clientSessionObjects.UserCmdGenerator.SetUserCmd(cmd => cmd.IsInterrupt = true);
                    }
                    if (IsReady()) SetGunSightFlag(value);
                    _enable = value;
                }
            }
        }

        public bool IsBagSwitchEnabled()
        {
            return null != Player && Player.modeLogic.ModeLogic.IsBagSwithEnabled(Player);
        }

        private void SetGunSightFlag(bool val)
        {
            if(val)
            {
                Player.playerInterruptState.ForceInterruptGunSight |= (int)InterruptReason.BagUI;
            }
            else
            {
                Player.playerInterruptState.ForceInterruptGunSight &= ~(int)InterruptReason.BagUI;
            }
        }

        public int RemainOperating
        {
            get
            {
                if(null == Player)
                {
                    return 0;
                }
                if(!Player.hasWeaponState)
                {
                    return 0;
                }
                return (Player.weaponState.BagOpenLimitTime - Player.time.ClientTime) / 1000;
            }
        }

        public IWeaponBagInfo GetWeaponBagInfoByBagIndex(int index)
        {
            return BagArray[index - 1];
        }

        private int _curBagIndex = 0;
        public int CurBagIndex
        {
            get
            {
                if(null == Player)
                {
                    return 0;
                }
                return Player.weaponState.BagIndex + 1;
            }
            set
            {
                if(null == Player)
                {
                    return;
                }
				if (value == CurBagIndex)
                {
                    return;
                }
                _contexts.session.clientSessionObjects.UserCmdGenerator.SetUserCmd((cmd) => cmd.BagIndex = value);
            }
        }

        public bool CanOpenBag
        {
            get
            {
                if (null != Player && Player.modeLogic.ModeLogic.IsBagSwithEnabled(Player))
                {
                    return true;
                }
                return false;
            }
        }

        public void SwitchBagByBagIndex(int index)
        {
            CurBagIndex = index;
        }
      
        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }
    }

    public class WeaponBagInfo : IWeaponBagInfo
    {
        private readonly NewWeaponConfigItem _newWeaponConfigItem;
        private readonly WeaponAvatarConfigItem _weaponAvatarConfigItem;
        private readonly WeaponBagItemInfo[] _weaponBagInfos = new WeaponBagItemInfo[7];
        private PlayerEntity _playerEntity;
        private const int StartIndex = 0;
        private Dictionary<int, WeaponBagItemInfo> _overrideWeaponItemInfoMap = new Dictionary<int, WeaponBagItemInfo>();

        public WeaponBagInfo(PlayerWeaponBagData playerWeaponBagData, PlayerEntity player)
        {
            _playerEntity = player;
            if(null == playerWeaponBagData)
            {
                return;
            }
            foreach(var weapon in playerWeaponBagData.weaponList)
            {
                _weaponBagInfos[weapon.Index - 1] = MakeWeaponBagItemInfo(weapon);
            }
        }

        private WeaponBagItemInfo MakeWeaponBagItemInfo(PlayerWeaponData weaponData)
        {
            var weaponId = weaponData.WeaponTplId;
            var weaponAvatarId = weaponData.WeaponAvatarTplId;
            var weaponConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponData.WeaponTplId);
            if(weaponAvatarId < 1)
            {
                weaponAvatarId = weaponConfig.AvatorId; 
            }
            return new WeaponBagItemInfo(weaponData, weaponConfig, SingletonManager.Get<WeaponAvatarConfigManager>().GetConfigById(weaponAvatarId));
        }

        public IWeaponBagItemInfo PrimeWeapon
        {
            get { return _weaponBagInfos[StartIndex]; }
        }

        public IWeaponBagItemInfo SubWeapon
        {
            get { return _weaponBagInfos[StartIndex + 1]; }
        }

        public IWeaponBagItemInfo MeleeWeapon
        {
            get { return _weaponBagInfos[StartIndex + 2]; }
        }

        public IWeaponBagItemInfo TacticalWeapon
        {
            get
            {
                if(null != _playerEntity && _playerEntity.hasOverrideBag && _playerEntity.overrideBag.TacticWeapon > 0)
                {
                    if (!_overrideWeaponItemInfoMap.ContainsKey(_playerEntity.overrideBag.TacticWeapon))
                    {
                        _overrideWeaponItemInfoMap[_playerEntity.overrideBag.TacticWeapon] = MakeWeaponBagItemInfo(
                            new PlayerWeaponData
                            {
                                WeaponTplId = _playerEntity.overrideBag.TacticWeapon,
                            });
                    }
                    return _overrideWeaponItemInfoMap[_playerEntity.overrideBag.TacticWeapon];
                }
                return _weaponBagInfos[StartIndex + 6];
            }
        }

        public IWeaponBagItemInfo GetGrenadeInfoByIndex(int index)
        {
            return _weaponBagInfos[index + StartIndex + 2];
        }
    }
}
