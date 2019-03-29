using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.ViewModels.Blast;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Blast
{

    public class BlastScoreModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BlastScoreModel));
        private BlastScoreViewModel _viewModel = new BlastScoreViewModel();

        private IBlastScoreUiAdapter _adapter;

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

        private Dictionary<EUICampType, List<PlayerItem>> campPlayerListDict = new Dictionary<EUICampType, List<PlayerItem>>(2);

        private void InitGui()
        {
            InitPlayerItemGroup();
            _viewModel.C4TipGroupShow = false;
        }

        private void InitPlayerItemGroup()
        {
            if (isInitPlayerItemGroup) return;
            campPlayerListDict[EUICampType.T] = new List<PlayerItem>();
            campPlayerListDict[EUICampType.CT] = new List<PlayerItem>();
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                campPlayerListDict[EUICampType.T].Add(GetNewPlayerItem(_campRoot1));
                campPlayerListDict[EUICampType.CT].Add(GetNewPlayerItem(_campRoot2));
            }

            campPlayerListDict[EUICampType.T].Reverse();

            isInitPlayerItemGroup = true;
        }

        private int MaxPlayerCount
        {
            get { return 8; }
        }

        private PlayerItem GetNewPlayerItem(Transform parent)
        {
            var tf = GameObject.Instantiate(_playerItem, parent);
            tf.gameObject.SetActive(true);
            var item = new PlayerItem();
            item.Root = tf.gameObject;
            item.Empty = tf.Find("Empty").gameObject;
            item.Empty.SetActive(true);
            item.Dead = tf.Find("Dead").gameObject;
            item.Dead.SetActive(false);
            item.Normal = tf.Find("Normal").gameObject;
            item.Normal.SetActive(false);
            return item;
        }

        private Transform _campRoot1;
        private Transform _campRoot2;
        private Transform _playerItem;

        class PlayerItem
        {
            public GameObject Empty;
            public GameObject Dead;
            public GameObject Normal;
            public GameObject Root;
        }
        
        private void InitVariable()
        {
            _campRoot1 = FindComponent<Transform>("PlayerGroup1");
            _campRoot2 = FindComponent<Transform>("PlayerGroup2");
            _playerItem = FindComponent<Transform>("PlayerItem");
            _playerItem.gameObject.SetActive(false);
        }

        public BlastScoreModel(IBlastScoreUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }
        bool isInitPlayerItemGroup = false;
        public override void Update(float interval)
        {
            //InitPlayerItemGroup();

            UpdateBombState();
            UpdateCampData();
            UpdateRemainTime();
            UpdateRoundInfo();
            UpdateBombShow();
        }

        private void UpdateBombShow()
        {
            if (_needResetBombProgress && !_haveResetBombProgress)
            {
                _c4BeginTime = _curTime;
                _haveResetBombProgress = true;
            }
            if (_needShowBomb)
            {
                _viewModel.C4TipGroupShow = true;
                float progress = (float)(_curTime - _c4BeginTime) / BombTotalTime;
                //if (progress >= 1f)
                //{
                //    progress = 1f;
                //    _adapter.C4InstallState = EUIBombInstallState.Completed;
                //}
                //progress = progress > 1f ? 1f : progress;
                _viewModel.C4TipValue = progress;
            }
            else
            {
                _viewModel.C4TipGroupShow = false;
                _haveResetBombProgress = false;
            }
        }

        private void UpdateBombState()
        {
            var state = _adapter.C4InstallState;

            _needShowBomb = state != EUIBombInstallState.None;
            //_needPause = state == EUIBombInstallState.Completed;
            _needPause = state != EUIBombInstallState.None || _adapter.NeedPause; 
            _needResetBombProgress = state != EUIBombInstallState.None;

        }

        private void UpdateRoundInfo()
        {
            _viewModel.ScoreText = _adapter.ScoreForWin.ToString();
        }

        private int _totalTime;
        private long _curTime, _beginTime;
        private bool _needPause = false;
        private long _c4BeginTime;
        private bool _needShowBomb = false;
        private bool _needResetBombProgress = false;
        private bool _haveResetBombProgress = false;
        

        private long BombTotalTime
        {
            get { return 35; }
        }
        private void UpdateRemainTime()
        {
            _curTime = DateTime.Now.Ticks / 10000000;

            if (_needPause)
            {
                return;
            }
            var newTotalTime = _adapter.GameTime / 1000;
            if (newTotalTime == 0)
            {
                _viewModel.TimeText = "00.00";
                return;
            }
            if (newTotalTime > 0)
            {
                _beginTime = DateTime.Now.Ticks / 10000000;//重置时间
                _totalTime = newTotalTime;
                _adapter.GameTime = -1000;//取值后设置为无效状态
            }
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

        private void UpdateCampData(EUICampType type)
        {
            var data = _adapter.GetDataByCampType(type);
            var itemList = campPlayerListDict[type];
            int index = 0;
            for (; index < data.PlayerCount - data.DeadPlayerCount && index < MaxPlayerCount; index++)
            {
                var item = itemList[index];
                item.Normal.SetActive(true);
                item.Dead.SetActive(false);
                item.Root.SetActive(true);
            }
            for (; index < data.PlayerCount && index < MaxPlayerCount; index++)
            {
                var item = itemList[index];
                item.Normal.SetActive(false);
                item.Dead.SetActive(true);
                item.Root.SetActive(true);
            }

            for (; index < _adapter.PlayerCapacityPerCamp && index < MaxPlayerCount; index++)
            {
                var item = itemList[index];
                item.Dead.SetActive(false);
                item.Normal.SetActive(false);
                item.Root.SetActive(true);
            }

            for (; index < MaxPlayerCount; index++)
            {
                var item = itemList[index];
                item.Root.SetActive(false);
            }
        }
    }
}


