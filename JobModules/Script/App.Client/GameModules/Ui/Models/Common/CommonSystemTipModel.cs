using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using System;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonSystemTipModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonSystemTipModel));
        private CommonMessageViewModel _viewModel = new CommonMessageViewModel();
        private ISystemTipUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonSystemTipModel(ISystemTipUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
        }


        private void InitGui()
        {
        }

        private void InitVariable()
        {

        }


        public override void Update(float intervalTime)
        {
            UpdateTipQueue();
            UpdateRemainTime();
            UpdateCurShowTip();
        }

        private void UpdateCurShowTip()
        {
            var tipList = _adapter.SystemTipDataQueue;

            if (null == tipList || tipList.Count == 0)
            {
                _curTipData = null;
                _adapter.Enable = false;
                return;
            }
            var item = tipList.Peek();
            _viewModel.Content = item.Title;
        }

        private void UpdateRemainTime()
        {
            var tipList = _adapter.SystemTipDataQueue;
            if (tipList.Count == 0)
            {
                return;
            }
            var curTime = DateTime.Now.Ticks / 10000;
            var item = tipList.Peek();
            if ((curTime - _createTime) > item.DurationTime)
            {
                tipList.Dequeue();
            }
        }

        private void UpdateTipQueue()
        {
            var dataList = _adapter.SystemTipDataQueue;
            if (dataList.Count == 0)
            {
                return;
            }

            var data = dataList.Peek();
            if (data == _curTipData) return;
            _curTipData = data;
            var curTime = DateTime.Now.Ticks / 10000;
            _createTime = curTime;
        }

        private long _createTime;
        private ITipData _curTipData;


    }
}
