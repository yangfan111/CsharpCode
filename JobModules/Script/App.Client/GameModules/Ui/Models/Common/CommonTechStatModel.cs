using System;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Utils.AssetManager;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Statistics;
using Core.Utils;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonTechStatModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonTechStatModel));
        private CommonTechStatViewModel _viewModel = new CommonTechStatViewModel();

        private ITechStatUiAdapter _adapter;

        private Transform _infoItemModel;
        private Transform _infoItemRoot;

        private Transform _curInfoItem;

        private string _badgeIconBundleName;

        private bool _initPanel;
        private DateTime _initPanelTime;
        private string _handAssetName;
        private Image _deathTypeIconImage;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            _infoItemRoot = FindChildGo("HurtInfoContent");
            _infoItemModel = FindChildGo("HurtInfo");
            _infoItemModel.gameObject.SetActive(false);
            _deathTypeIconImage = FindChildGo("DeathTypeIcon").GetComponent<Image>();
            _handAssetName = "HandKill";
        }


        public CommonTechStatModel(ITechStatUiAdapter techStatState):base(techStatState)
        {
            _adapter = techStatState;
            _adapter.Enable = false;
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);
            if (enable)
            {
                _initPanel = false;
            }
            else
            {
                ClearOldHurtInfo();
            }
        }
        public override void Update(float interval)
        {
            
            if (!_initPanel)
            {
                _initPanel = true;
                _initPanelTime = DateTime.Now;
                GetKillerCardInfo();
                GetHurtInfo();
            }
            else if ((DateTime.Now - _initPanelTime).TotalMilliseconds > 5000)
            {
                //_adapter.Enable = false;
            }
            
        }
        /// <summary>
        /// 获取当前玩家造成的伤害信息
        /// </summary>
        private void GetHurtInfo()
        {
            //ClearOldHurtInfo();
            foreach (var info in _adapter.BattleData.OpponentList)
            {
                _curInfoItem = NewHurtInfoItem();
                GetNewHurtInfo(info);
                _curInfoItem.gameObject.SetActive(true);
            }
        }

        private void ClearOldHurtInfo()
        {
            if (null != _infoItemRoot)
            {
                UITool.HideChilds(_infoItemRoot);
            }
        }

        /// <summary>
        /// 获取击杀方军牌信息
        /// </summary>
        private void GetKillerCardInfo()
        {
            PlayerBattleInfo killer = _adapter.BattleData.Killer;
            var deathType = killer.DeadType;
            var weaponId = killer.WeaponId;
            Logger.InfoFormat("deathType={0},weaponId={1}", deathType, weaponId);
            //Debug.Log(deathType);
            if (deathType <= 0)
            {
                _adapter.Enable = false;
                return;
            }
            UpdateCardInfo(deathType);
            UpdateDeathTypeInfo(deathType, weaponId);
        }

        private void UpdateDeathTypeInfo(EUIDeadType type, int weaponId)
        {
            switch (type)
            {
                case EUIDeadType.Weapon:
                    ShowWeaponIcon(weaponId);
                    break;
                case EUIDeadType.Unarmed:
                    ShowHandIcon();
                    break;
                case EUIDeadType.VehicleHit:
                    ShowVehicleIcon(weaponId);
                    break;
                case EUIDeadType.VehicleBomb:
                    if (weaponId > 0)
                    {
                        ShowVehicleIcon(weaponId);
                    }
                    else
                    {
                        SetDeathIconView(false);
                    }
                    break;
                default:
                    SetDeathIconView(false);
                    break;
            }
            
        }

        private void ShowVehicleIcon(int vehicleId)
        {
            AssetInfo assetInfo = new AssetInfo(AssetBundleConstant.Icon_Carry, "carry_" + vehicleId);
            Loader.RetriveSpriteAsync(assetInfo.BundleName, assetInfo.AssetName,
                (sprite) => { _viewModel.DeathTypeIconSprite = sprite; });
            SetDeathIconView(true);
        }

        private void ShowWeaponIcon(int weaponId)
        {
            WeaponAvatarConfigItem config = SingletonManager.Get<WeaponAvatarConfigManager>().GetConfigById(weaponId);
            if (null != config)
            {
                Loader.RetriveSpriteAsync(config.IconBundle, config.Icon,
                    (sprite) => { _viewModel.DeathTypeIconSprite = sprite; });
            }

            SetDeathIconView(true);
        }

        private void SetDeathIconView(bool val)
        {
            _viewModel.DeathTypeGroupShow = val;
            if (val)
            {
                SetDeathIconSuitable();
            }
        }

        private void SetDeathIconSuitable()
        {
            UIUtils.SetImageSuitable(_deathTypeIconImage);
        }

        private void ShowHandIcon()
        {
            Loader.RetriveSpriteAsync(AssetBundleConstant.Icon_Weapon, _handAssetName,
                (sprite) => { _viewModel.DeathTypeIconSprite = sprite; });
            SetDeathIconView(true);
        }

        private void UpdateCardInfo(EUIDeadType type)
        {
            switch (type)
            {
                case EUIDeadType.VehicleHit:
                case EUIDeadType.NoHelp:
                case EUIDeadType.Weapon:
                case EUIDeadType.Unarmed:
                    UpdatePlayerCard();
                    break;
                default:
                    UpdateNonCardById((int) type);
                    break;
            }
        }

        private void UpdateNonCardById(int id)
        {
            var cfg = SingletonManager.Get<TypeForDeathConfigManager>();
            var item = cfg.GetConfigById(id);
            Loader.RetriveSpriteAsync(item.BundleName, item.CardName,
                (sprite) => { _viewModel.BadgeIconSprite = sprite; });
            _viewModel.KillerNameString = item.Name;
            
            SetPlayerSpecicalInfoView(false);
        }

        private void UpdatePlayerCard()
        {
            PlayerBattleInfo killer = _adapter.BattleData.Killer;

            _viewModel.KillerLvString = killer.PlayerLv.ToString();
            _viewModel.KillerNameString = killer.PlayerName;
            AssetInfo badgeAssetInfo = GetBadgeAssetInfo();
            Loader.RetriveSpriteAsync(badgeAssetInfo.BundleName, badgeAssetInfo.AssetName,
                (sprite) => { _viewModel.BadgeIconSprite = sprite; });
            AssetInfo bgAssetInfo = GetCardBgAssetInfo();
            Loader.RetriveSpriteAsync(bgAssetInfo.BundleName, bgAssetInfo.AssetName,
                (sprite) => { _viewModel.KillerCardBgSprite = sprite; });
            AssetInfo titleAssetInfo = GetTitleAssetInfo();
            Loader.RetriveSpriteAsync(titleAssetInfo.BundleName, titleAssetInfo.AssetName,
                (sprite) => { _viewModel.KillerTitleSprite = sprite; });
            SetPlayerSpecicalInfoView(true);
        }

        private void SetPlayerSpecicalInfoView(bool val)
        {
            _viewModel.KillerLvShow = val;
            _viewModel.KillerTitleShow = val;
            _viewModel.KillerCardBgShow = val;
        }

        private AssetInfo GetTitleAssetInfo()
        {
            return SingletonManager.Get<CardConfigManager>().GetAssetInfoById(_adapter.BattleData.Killer.TitleId);
        }

        private AssetInfo GetCardBgAssetInfo()
        {
            return SingletonManager.Get<CardConfigManager>().GetAssetInfoById(_adapter.BattleData.Killer.BackId);
        }

        private AssetInfo GetBadgeAssetInfo()
        {
            return SingletonManager.Get<CardConfigManager>().GetAssetInfoById(_adapter.BattleData.Killer.BadgeId);
        }

        private void GetNewHurtInfo(OpponentBattleInfo info)
        {
            _curInfoItem.Find("OpponentName").GetComponent<Text>().text = info.PlayerName;
            _curInfoItem.Find("IsKillIcon").gameObject.SetActive(info.IsKill);
            _curInfoItem.Find("HurtVal").GetComponent<Text>().text = info.Damage.ToString();
            _curInfoItem.gameObject.SetActive(true);
        }

        private Transform NewHurtInfoItem()
        {
            return UnityEngine.Object.Instantiate(_infoItemModel, _infoItemRoot);      
        }
    }
}
