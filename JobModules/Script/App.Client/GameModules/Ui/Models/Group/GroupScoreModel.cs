using System;
using App.Client.GameModules.Ui.ViewModels.Group;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Group
{

    public class GroupScoreModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GroupScoreModel));
        private GroupScoreViewModel _viewModel = new GroupScoreViewModel();
        private Contexts _contexts;
        private IGroupScoreUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
        }


        private void InitVariable()
        {
        }



        public GroupScoreModel(IGroupScoreUiAdapter groupScoreState):base(groupScoreState)
        {
            _adapter = groupScoreState;
        }
        float _interval;
        public override void Update(float interval)
        {
            if (!isVisible || !_viewInitialized) return;

            this._interval = interval;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            UpdateKillCount();
            UpdateRemainTime();
            UpdateWinCondition();
        }

        private void UpdateWinCondition()
        {
            int count = _adapter.KillCountForWin;
            if (count <= 0)
            {
                return;
            }
            _viewModel.WinContionText = count.ToString();
            _viewModel.WinContionColor = Color.white;
        }

        private int totalTime;
        long curTime,beginTime;
        private void UpdateRemainTime()
        {
            if (_adapter.NeedPause)
            {
                return;
            }
            int count = _adapter.KillCountForWin;
            if (count > 0)
            {
                return;
            }

            var newTotalTime = _adapter.GameTime / 1000;
            if (newTotalTime == 0)
            {
                _viewModel.WinContionText = "00.00";
                return;
            }
            if (newTotalTime > 0)
            {
                beginTime = DateTime.Now.Ticks / 10000000;//重置时间
                totalTime = newTotalTime;
                _adapter.GameTime = -1000;//取值后设置为无效状态
            }
            curTime = DateTime.Now.Ticks / 10000000;
            int remainTime = (int)(beginTime - curTime + totalTime);
            remainTime = remainTime < 0 ? 0 : remainTime;
            int minute = remainTime / 60;
            int second = remainTime % 60;
            string minuteStr = minute < 10 ? "0" + minute : minute.ToString();
            string secondStr = second < 10 ? "0" + second : second.ToString();

            //_viewModel.WinContionText = string.Format("{0}.{1}", minute, second);
            _viewModel.WinContionText = minuteStr + "." + secondStr;
            _viewModel.WinContionColor = minute < 1 ? Color.red : Color.white;
 
        }

        private void UpdateKillCount()
        {
            _viewModel.CampKillCountText1 = _adapter.GetKillCountByCampType(EUICampType.T).ToString();
            _viewModel.CampKillCountText2 = _adapter.GetKillCountByCampType(EUICampType.CT).ToString();
        }

    }
}

