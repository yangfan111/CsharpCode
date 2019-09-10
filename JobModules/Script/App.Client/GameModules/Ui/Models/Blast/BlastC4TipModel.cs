using System;
using App.Client.GameModules.Ui.UiAdapter.Interface.Blast;
using App.Client.GameModules.Ui.ViewModels.Blast;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;

namespace App.Client.GameModules.Ui.Models.Blast
{
    public class BlastC4TipModel : ClientAbstractModel, IUiHfrSystem
    {
        protected LoggerAdapter Logger = new LoggerAdapter(typeof(BlastC4TipModel));
        protected BlastC4TipViewModel _viewModel = new BlastC4TipViewModel();

        private IBlastC4TipUiAdapter _adapter;

        private bool _needResetBombProgress = false;
        private bool _haveResetBombProgress = false;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public BlastC4TipModel(IBlastC4TipUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }
        public override void Update(float interval)
        {
            UpdateCurTime();
            UpdateBombShow();
        }

        private void UpdateCurTime()
        {
            _curTime = DateTime.Now.Ticks / 10000000;
        }

        private void UpdateBombShow()
        {
            float progress = (float)(_curTime - _c4BeginTime) / BombTotalTime + _adapter.C4InitialProgress;
            _viewModel.C4TipValue = progress;
        }

        private long _curTime;
        private long _c4BeginTime;


        private const long BombTotalTime = 35;

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);
            if (!enable)
            {
                UpdateResetState();
            }
            else
            {
                ResetBeginTime();
            }
        }

        private void UpdateResetState()
        {
            if (_adapter.C4InstallState == Core.Enums.EUIBombInstallState.None)
            {
                _needResetBombProgress = true;
            }
        }

        private void ResetBeginTime()
        {
            if (!_needResetBombProgress) return;
            UpdateCurTime();
            _c4BeginTime = _curTime;
        }
    }
}
