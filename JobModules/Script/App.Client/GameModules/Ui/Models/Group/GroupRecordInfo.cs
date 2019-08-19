using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Group;
using Assets.UiFramework.Libs;
using Core.Enums;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Group
{

    public class GroupRecordInfo : AbstractRecordInfo
    {
        private GroupRecordInfoViewModel _viewModel = new GroupRecordInfoViewModel();


        protected override IUiViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        protected override void UpdateSingleItemBadge(GroupRecordViewData data)
        {
            var battleData = data.BattleData;
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
                if (string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName))
                {
                    _viewModel.BadgeGroupShow = false;
                    return;
                }

                _viewModel.BadgeIconBundle = assetInfo.BundleName;
                _viewModel.BadgeIconAsset = assetInfo.AssetName;
                _viewModel.BadgeGroupShow = true;
            }

            _viewModel.BadgeNormalBgShow = !battleData.IsMySelf;
            _viewModel.BadgeMySelfBgShow = battleData.IsMySelf;
            _viewModel.BadgeHurtBgShow = battleData.IsHurt;
            _viewModel.BadgeDeadBgShow = battleData.IsDead;

        }

        protected override void RealUpdateGroupShow(GroupRecordViewData data)
        {
            _viewModel.ImgGroupShow = !data.IsTitle;
            _viewModel.TextGroupShow = data.NeedShow;
            _viewModel.IconGroupShow = data.NeedShow && !data.IsTitle;
        }

        protected override void InitTitleText(bool canRescue)
        {
            _viewModel.RankText = I2.Loc.ScriptLocalization.client_group.word1;
            _viewModel.PlayerNameText = I2.Loc.ScriptLocalization.client_group.word2;
            _viewModel.CorpsText = I2.Loc.ScriptLocalization.client_group.word3;
            _viewModel.KillText = canRescue ? I2.Loc.ScriptLocalization.client_group.EliminateAndHitDown : I2.Loc.ScriptLocalization.client_group.Eliminate;
            _viewModel.DamageText = I2.Loc.ScriptLocalization.client_group.word5;
            _viewModel.DeadText = I2.Loc.ScriptLocalization.client_group.word6;
            _viewModel.AssistText = canRescue ? I2.Loc.ScriptLocalization.client_group.AssistAndResque : I2.Loc.ScriptLocalization.client_group.word7;
        }


        protected override void UpdateColor(Color color)
        {
            _viewModel.RankColor = color;
            _viewModel.PlayerNameColor = color;
            _viewModel.CorpsColor = color;
            _viewModel.KillColor = color;
            _viewModel.DamageColor = color;
            _viewModel.DeadColor = color;
            _viewModel.AssistColor = color;
        }

        protected override void UpdateSingleItemIcon(GroupRecordViewData viewData)
        {
            var data = viewData.BattleData;
            _viewModel.DeadStateShow = data.IsDead;
            _viewModel.HurtStateShow = data.IsHurt;
            _viewModel.MySelfMaskShow = data.IsMySelf;
            int gameTitle = data.GameTitle;
            _viewModel.TitleIconShow1 = IsTitle(EUIGameTitleType.Ace, gameTitle);
            _viewModel.TitleIconShow2 = IsTitle(EUIGameTitleType.Second, gameTitle);
            _viewModel.TitleIconShow3 = IsTitle(EUIGameTitleType.Third, gameTitle);
            _viewModel.TitleIconShow4 = IsTitle(EUIGameTitleType.KdKing, gameTitle);
        }

        protected override void UpdateSingleItemData(GroupRecordViewData viewData)
        {
            var canResque = viewData.CanResque;
            var data = viewData.BattleData;
            _viewModel.AssistText = canResque ? 
                string.Format(twoDataFormat,data.AssistCount,data.ResqueCount):data.AssistCount.ToString();
            _viewModel.CorpsText = data.CorpsName;
            _viewModel.DamageText = data.Damage.ToString();
            _viewModel.DeadText = data.DeadCount.ToString();
            _viewModel.KillText = canResque ? 
                string.Format(twoDataFormat,data.HitDownCount, data.KillCount):data.KillCount.ToString();
            _viewModel.PlayerNameText = data.Name;
            _viewModel.RankText = data.IsHurt || data.IsDead ? string.Empty : viewData.Rank.ToString();
        }

    }
}
