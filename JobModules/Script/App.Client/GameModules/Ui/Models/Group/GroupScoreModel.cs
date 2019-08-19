using System;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Group;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using I2.Loc;
using UIComponent.UI;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Group
{
    public class GroupScoreModel : ClientAbstractModel, IUiHfrSystem
    {
        protected LoggerAdapter Logger = new LoggerAdapter(typeof(GroupScoreModel));
        protected GroupScoreViewModel _viewModel = new GroupScoreViewModel();

        private IGroupScoreUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
        }

        protected void InitGui()
        {
            _viewModel.TimeShow = NeedUpdateTime;
        }

        UIList TList, CTList;


        private void InitVariable()
        {
            TList = FindComponent<UIList>("PlayerGroup1");
            CTList = FindComponent<UIList>("PlayerGroup2");
        }

        public GroupScoreModel(IGroupScoreUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        public override void Update(float interval)
        {
            UpdateState();
            UpdateWinCodition();
            UpdateCampData();
            UpdateRemainTime();
        }

        private void UpdateState()
        {
            _needPause = _adapter.NeedPause;
        }

        private int _scoreForWin = -1;

        private void UpdateWinCodition()
        {
            if (_scoreForWin == _adapter.ScoreForWin)
            {
                return;
            }
            _scoreForWin = _adapter.ScoreForWin;
            RealUpdateWinCodition(_scoreForWin);
        }

        protected virtual void RealUpdateWinCodition(int score)
        {
            var format = ScriptLocalization.client_blast.WinKillFormat;
            _viewModel.ScoreText = NeedUpdateTime ? string.Empty : string.Format(format, score.ToString());
            if (!NeedUpdateTime)
            {
                _viewModel.TimeText = string.Empty;
            }
        }

        private int _totalTime;
        private long _curTime, _beginTime;
        private bool _needPause = false;

        protected virtual bool NeedUpdateTime
        {
            get
            {
                return _adapter != null && _adapter.ScoreForWin <= 0;
            }
        }

        protected void UpdateRemainTime()
        {
            if (!NeedUpdateTime) return;

            var newTotalTime = _adapter.GameTime / 1000;
            if (newTotalTime == 0)
            {
                _viewModel.TimeText = "00.00";
                return;
            }

            if (newTotalTime > 0)
            {
                _beginTime = DateTime.Now.Ticks / 10000000; //重置时间
                _totalTime = newTotalTime;
                _adapter.GameTime = -1000; //取值后设置为无效状态
                RefreshTime();
            }

            if (_needPause) return;
            RefreshTime();
        }

        private void RefreshTime()
        {
            _curTime = DateTime.Now.Ticks / 10000000;
            int remainTime = (int)(_beginTime - _curTime + _totalTime);
            remainTime = remainTime < 0 ? 0 : remainTime;
            int minute = remainTime / 60;
            int second = remainTime % 60;
            string minuteStr = minute < 10 ? "0" + minute : minute.ToString();
            string secondStr = second < 10 ? "0" + second : second.ToString();

            _viewModel.TimeText = minuteStr + "." + secondStr;
            _viewModel.TimeColor = minute < 1 && second < 30 ? Color.red : Color.white;
        }

        private void UpdateCampData()
        {
            UpdateCampData(EUICampType.CT);
            UpdateCampData(EUICampType.T);
            UpdateScore();
        }

        private void UpdateScore()
        {
            _viewModel.CampKillCountText1 = _adapter.GetScoreByCamp(EUICampType.T).ToString();
            _viewModel.CampKillCountText2 = _adapter.GetScoreByCamp(EUICampType.CT).ToString();
        }

        UIList GetUIListByCampType(EUICampType type)
        {
            switch (type)
            {
                case EUICampType.CT: return CTList;
                case EUICampType.T: return TList;
                default: return null;
            }
        }

        private void UpdateCampData(EUICampType type)
        {
            var dataList = _adapter.GetBattleDataListByCampType(type);
            var uiList = GetUIListByCampType(type);
            if (uiList == null)
            {
                Logger.Error("Not found UIList With Camp" + type);
            }

            uiList.SetDataList<PlayerBadgeItem, IGroupBattleData>(dataList);
        }
    }
}

