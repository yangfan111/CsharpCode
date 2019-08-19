using System;
using App.Client.GameModules.Ui.ViewModels.Group;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.Utils;
using UIComponent.UI;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Group
{

    public class PlayerBadgeItem : UIItem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBadgeItem));

        private PlayerBadgeItemViewModel _viewModel = new PlayerBadgeItemViewModel();

        protected override void SetView()
        {
            base.SetView();

            RefreshGui();
        }


        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        private int curBadgeId;

        private void RefreshGui()
        {
            var battleData = Data as IGroupBattleData;
            if (battleData == null)
            {
                _viewModel.BadgeGroupShow = false;
                return;
            }
            var badgeId = battleData.BadgeId;

            if (badgeId <= 0)
            {
                _viewModel.BadgeGroupShow = false;
                return;
            }
            if (badgeId != curBadgeId)
            {
                curBadgeId = badgeId;
                var assetInfo = GetBadgeAssetInfo(badgeId);
                if(string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName))
                {
                    _viewModel.BadgeGroupShow = false;
                    return;
                }
                _viewModel.BadgeGroupShow = true;
                _viewModel.BadgeIconBundle = assetInfo.BundleName;
                _viewModel.BadgeIconAsset = assetInfo.AssetName;
            }

            _viewModel.BadgeNormalBgShow = !battleData.IsMySelf;
            _viewModel.BadgeMySelfBgShow = battleData.IsMySelf;
            _viewModel.BadgeHurtBgShow = battleData.IsHurt;
            _viewModel.BadgeDeadBgShow = battleData.IsDead;
        }

        private void ResetImg()
        {
            _viewModel.BadgeGroupShow = false;
            curBadgeId = 0;
        }

        private AssetInfo GetBadgeAssetInfo(int id)
        {
            return SingletonManager.Get<CardConfigManager>().GetAssetInfoById(id);
        }

        protected override void OnAddToPool()
        {
            base.OnAddToPool();
            ResetImg();
        }
    }
}
