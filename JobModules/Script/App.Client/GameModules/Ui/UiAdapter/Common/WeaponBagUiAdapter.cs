﻿using App.Shared;
using App.Shared.Components.Ui.UiAdapter;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core.Room;
using Core.Utils;
using System.Collections.Generic;
using App.Shared.GameModules.Player;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;


namespace App.Client.GameModules.Ui.UiAdapter
{

    public class WeaponBagItemInfo : IWeaponBagItemInfo
    {
        private readonly WeaponResConfigItem _weaponConfig;
        private readonly WeaponAvatarConfigItem _weaponAvatarConfig;
        private readonly PlayerWeaponData _playerWeaponData;
        private readonly AssetInfo _magazineAsset; 
        private readonly AssetInfo _muzzleAsset; 
        private readonly AssetInfo _stockAsset; 
        private readonly AssetInfo _upperAsset; 
        private readonly AssetInfo _lowerAsset;

        private AssetInfo GetWeaponPartIcon(int id)
        {
            var config = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(id);
            if (config == null) return AssetInfo.EmptyInstance;
            return new AssetInfo(config.IconBundle, config.Icon);
        }

        public WeaponBagItemInfo(PlayerWeaponData playerWeaponData, WeaponResConfigItem newWeaponConfig, WeaponAvatarConfigItem weaponAvatarConfigItem)
        {
            _playerWeaponData = playerWeaponData;
            _weaponConfig = newWeaponConfig;
            _weaponAvatarConfig = weaponAvatarConfigItem;
            if (_playerWeaponData.Muzzle > 0)
            {
                _muzzleAsset = GetWeaponPartIcon(_playerWeaponData.Muzzle);
            }
            if (_playerWeaponData.Magazine > 0)
            {
                _magazineAsset = GetWeaponPartIcon(_playerWeaponData.Magazine);
            }
            if (_playerWeaponData.Stock > 0)
            {
                _stockAsset = GetWeaponPartIcon(_playerWeaponData.Stock);
            }
            if (_playerWeaponData.UpperRail > 0)
            {
                _upperAsset = GetWeaponPartIcon(_playerWeaponData.UpperRail);
            }
            if (_playerWeaponData.LowerRail > 0)
            {
                _lowerAsset = GetWeaponPartIcon(_playerWeaponData.LowerRail);
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
                return _weaponConfig.Name;
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

        public int GetIdByWeaponPartType(EWeaponPartType weaponPartType)
        {
            switch (weaponPartType)
            {
                case EWeaponPartType.LowerRail:
                    return _playerWeaponData.LowerRail;
                case EWeaponPartType.UpperRail:
                    return _playerWeaponData.UpperRail;
                case EWeaponPartType.Stock:
                    return _playerWeaponData.Stock;
                case EWeaponPartType.Magazine:
                    return _playerWeaponData.Magazine;
                case EWeaponPartType.Muzzle:
                    return _playerWeaponData.Muzzle;
            }
            return 0;
        }

        public int id
        {
            get { return _weaponConfig.Id; }
        }
    }

    public class WeaponBagUiAdapter : UIAdapter, IWeaponBagUiAdapter
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagUiAdapter));
        private Contexts _contexts;

        WeaponBagInfo[] _bagArray;


        private WeaponBagInfo[] BagArray
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
                        if (bagInfo != null)
                        {
                            var uiIndex = bagInfo.BagIndex + 1;
                            _bagArray[uiIndex] = new WeaponBagInfo(bagInfo, playerEntity.WeaponController());
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
            return WeaponController != null;
        }

        public override bool Enable
        {
            get
            {
                return _enable && WeaponController.CanSwitchWeaponBag;
            }

            set {
                if (WeaponController != null && !WeaponController.CanSwitchWeaponBag)
                {
                    _enable = false;
                }
                else
                {
                    if (value)
                    {
                        _contexts.session.clientSessionObjects.UserCmdGenerator.SetUserCmd(cmd => cmd.IsInterrupt = true);
                        
                    }
                    _enable = value;
                }
                if(_enable)
                    PlayerStateUtil.AddUIState(EPlayerUIState.BagOpen,_contexts.player.flagSelfEntity.gamePlay);
                else
                    PlayerStateUtil.RemoveUIState(EPlayerUIState.BagOpen,_contexts.player.flagSelfEntity.gamePlay);
                    
            }
        }

       
        public int RemainOperating
        {
            get
            {
                return (WeaponController.BagOpenLimitTIme - WeaponController.RelatedTime) / 1000;
            }
        }

        public IWeaponBagInfo GetWeaponBagInfoByBagIndex(int index)
        {
            return BagArray[index];
        }
        private PlayerWeaponController _weaponController;
        private PlayerWeaponController WeaponController
        {
            get { return _weaponController ?? (_weaponController = _contexts.player.flagSelfEntity.WeaponController()); }
        }
        private int _curBagIndex = 0;
        public int CurBagIndex
        {
            get
            {
                if(null == _contexts.player.flagSelfEntity)
                {
                    return 0;
                }

                return WeaponController.HeldBagPointer+ 1;
            }
            set
            {
                if(null == _contexts.player.flagSelfEntity)
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
                return WeaponController.CanSwitchWeaponBag;
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
        private readonly WeaponResConfigItem _newWeaponConfigItem;
        private readonly WeaponAvatarConfigItem _weaponAvatarConfigItem;
        private readonly WeaponBagItemInfo[] _weaponBagInfos = new WeaponBagItemInfo[7];
        private PlayerWeaponController _controller;
        private const int StartIndex = 0;
        private Dictionary<int, WeaponBagItemInfo> _overrideWeaponItemInfoMap = new Dictionary<int, WeaponBagItemInfo>();

        public WeaponBagInfo(PlayerWeaponBagData playerWeaponBagData, PlayerWeaponController controller)
        {
            _controller = controller;
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
            var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponData.WeaponTplId);
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
                int tacticAmount = _controller.OverrideBagTactic;
                if (tacticAmount > 0)
                {
                    if (!_overrideWeaponItemInfoMap.ContainsKey(tacticAmount))
                    {
                        _overrideWeaponItemInfoMap[tacticAmount] = MakeWeaponBagItemInfo(
                            new PlayerWeaponData
                            {
                                WeaponTplId = tacticAmount,
                            });
                    }
                    return _overrideWeaponItemInfoMap[tacticAmount];
                }
                return _weaponBagInfos[StartIndex + 4];
            }
        }

        public IWeaponBagItemInfo GetGrenadeInfoByIndex(int index)
        {
            if (index != 1) return null;
            return _weaponBagInfos[index + StartIndex + 2];
        }
    }
}
