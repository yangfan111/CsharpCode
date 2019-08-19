using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UIComponent.UI;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonCountdownTipModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonCountdownTipModel));
        private CommonCountdownTipViewModel _viewModel = new CommonCountdownTipViewModel();
        private ICountdownTipUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonCountdownTipModel(ICountdownTipUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        private UISelectable uISelectable;

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
        }


        private void InitGui()
        {
            //_adapter.Enable = false;
        }

        private void InitVariable()
        {
            uISelectable = ViewInstance.GetComponentInChildren<UISelectable>();
        }


        public override void Update(float intervalTime)
        {
            UpdateTipQueue();
            UpdateRemainTime();
            UpdateCurShowTip();
        }

        private void UpdateCurShowTip()
        {
            var tipList = _adapter.CountdownTipDataList;

            if (null == tipList || tipList.Count == 0)
            {
                _adapter.Enable = false;
                return;
            }
            var curTime = _adapter.CurTime;
            var item = tipList[0];
            _viewModel.TitleText = item.Title;
            var remainTime = _createTimeDict[item] - curTime + item.DurationTime;
            remainTime = remainTime < 0 ? 0 : (remainTime + 1000) / 1000;
            _viewModel.TimeText = remainTime.ToString();
            if (uISelectable != null)
            {
                uISelectable.Selected = remainTime < timeNeedSwitchColor;//to make all text color to red;
            }
        }

        private const float timeNeedSwitchColor = 10f;

        private void UpdateRemainTime()
        {
            var tipList = _adapter.CountdownTipDataList;
            var curTime = _adapter.CurTime;
            for (int i = tipList.Count - 1; i >= 0; i--)
            {
                var item = tipList[i];
                if((curTime - _createTimeDict[item]) > item.DurationTime)
                {
                    tipList.RemoveAt(i);
                    _createTimeDict.Remove(item);
                }
            }    
        }

        private void UpdateTipQueue()
        {
            var dataList = _adapter.CountdownTipDataList;
            if(dataList.Count == 0)
            {
                return;
            }
            var curTime = _adapter.CurTime;
            foreach (var it in dataList)
            {
                if (!_createTimeDict.ContainsKey(it))
                {
                    _createTimeDict.Add(it, curTime);
                    Logger.Info("Add CountDown:" + it);
                }
            }
        }

        private Dictionary<ITipData, long> _createTimeDict = new Dictionary<ITipData, long>();

    }

}
