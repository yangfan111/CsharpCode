using System;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Blast;
using Assets.UiFramework.Libs;
using Core.Enums;
using UIComponent.UI;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Blast
{
    public class BlastRecordInfo : UIItem
    {
        private BlastRecordInfoViewModel _viewModel = new BlastRecordInfoViewModel();


        protected override IUiViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        protected override void SetView()
        {
            base.SetView();

            UpdateInfoView();
        }

        public void UpdateInfoView()
        {
            var data = Data as GroupRecordViewData;

            UpdateGroupShow(data);
            if (!_viewModel.IconGroupShow) return;
            UpdateSingleItemData(data);
            UpdateSingleItemIcon(data);
            UpdateSingleItemMySelf(data);
        }

        private void UpdateGroupShow(GroupRecordViewData data)
        {
            _viewModel.ImgGroupShow = !data.IsTitle;
            _viewModel.TextGroupShow = data.NeedShow;
            _viewModel.IconGroupShow = data.NeedShow && !data.IsTitle;
            if (data.IsTitle)
            {
                InitTitleText(data.CanResque);
                _viewModel.BombText = data.CampType == EUICampType.T ? I2.Loc.ScriptLocalization.client_blast.word3 : I2.Loc.ScriptLocalization.client_blast.word4;//埋雷 拆雷
            }
        }

        private void InitTitleText(bool canResque)
        {
            _viewModel.RankText = I2.Loc.ScriptLocalization.client_blast.word5;
            _viewModel.PlayerNameText = I2.Loc.ScriptLocalization.client_blast.word6;
            _viewModel.CorpsText = I2.Loc.ScriptLocalization.client_blast.word7;
            _viewModel.KillText = canResque ? I2.Loc.ScriptLocalization.client_blast.word12 : I2.Loc.ScriptLocalization.client_blast.word8;
            _viewModel.DamageText = I2.Loc.ScriptLocalization.client_blast.word9;
            _viewModel.DeadText = I2.Loc.ScriptLocalization.client_blast.word10;
            _viewModel.AssistText = I2.Loc.ScriptLocalization.client_blast.word11;
            _viewModel.PingText = "ping";
        }

        private Color origColor = new Color32(0xf4, 0xf4, 0xf4, 255);
        private Color myselfColor = new Color32(0xfd, 0xcd, 0x50, 255);
        private void UpdateSingleItemMySelf(GroupRecordViewData viewData)
        {
            var data = viewData.BattleData;
            if (data.IsMySelf)
            {
                UpdateColor(myselfColor);
                _viewModel.MySelfMaskShow = true;
            }
            else
            {
                UpdateColor(origColor);
                _viewModel.MySelfMaskShow = false;
            }
        }

        private void UpdateColor(Color color)
        {
            _viewModel.RankColor = color;
            _viewModel.PlayerNameColor = color;
            _viewModel.CorpsColor = color;
            _viewModel.KillColor = color;
            _viewModel.DamageColor = color;
            _viewModel.DeadColor = color;
            _viewModel.AssistColor = color;
            _viewModel.PingColor = color;
            _viewModel.BombColor = color;
        }

        private void UpdateSingleItemIcon(GroupRecordViewData viewData)
        {
            var data = viewData.BattleData;
            _viewModel.DeadIconShow = data.IsDead;
            _viewModel.DeathMaskShow = data.IsDead;
            _viewModel.MySelfMaskShow = data.IsMySelf;
            int gameTitle = data.GameTitle;
            _viewModel.TitleIconShow1 = IsTitle(EUIGameTitleType.KdKing, gameTitle);
            _viewModel.TitleIconShow2 = IsTitle(EUIGameTitleType.Ace, gameTitle);
            _viewModel.TitleIconShow3 = IsTitle(EUIGameTitleType.Second, gameTitle);
            _viewModel.TitleIconShow4 = IsTitle(EUIGameTitleType.Third, gameTitle);
            _viewModel.BombIconShow = data.HaveC4 && viewData.CanShowC4;
        }

        private bool IsTitle(EUIGameTitleType titleType, int title)
        {
            int type = 1 << (int)titleType;
            return (title & type) == type;
        }

        private void UpdateSingleItemData(GroupRecordViewData viewData)
        {
            var data = viewData.BattleData;
            _viewModel.AssistText = data.AssistCount.ToString();
            _viewModel.CorpsText = data.CorpsName;
            _viewModel.DamageText = data.Damage.ToString();
            _viewModel.DeadText = data.DeadCount.ToString();
            _viewModel.KillText = data.KillCount.ToString();
            _viewModel.PingText = data.Ping.ToString();
            _viewModel.PlayerNameText = data.Name;
            _viewModel.RankText = viewData.Rank.ToString();
            _viewModel.BombText = (data.C4PlantCount + data.C4DefuseCount).ToString();
        }

    }
}
