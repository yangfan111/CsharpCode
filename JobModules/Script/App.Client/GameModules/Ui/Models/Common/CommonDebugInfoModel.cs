using System;
using System.Runtime.InteropServices;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine.Profiling;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonDebugInfoModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonDebugInfoModel));
        private CommonDebugInfoViewModel _viewModel = new CommonDebugInfoViewModel();

        private IDebugInfoUiAdapter _adapter;
        private KeyReceiver KeyReceiver;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public long DisplayTime
        {
            get { return 10000000; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
            InitKey();
        }

        private void InitKey()
        {
            KeyReceiver = new KeyReceiver(Layer.System, BlockType.None);
            KeyReceiver.AddAction(UserInputKey.ShowDebug, (data) => { _adapter.Enable = !_adapter.Enable; });
            _adapter.RegisterKeyReceive(KeyReceiver);
        }

        private void InitGui()
        {

        }

        public CommonDebugInfoModel(IDebugInfoUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

   

        public override void Update(float interval)
        {
           
            if (DateTime.Now.Ticks - _lastTime < DisplayTime) return;
            _lastTime = DateTime.Now.Ticks;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            _viewModel.LeftInfoText = _adapter.VersionDebugInfo;
            _viewModel.RightInfoText = _adapter.PingDebugInfo;
        }

        private long _lastTime;
    }
}
