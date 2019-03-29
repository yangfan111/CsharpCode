using System;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonDeadModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonDeadModel));
        private CommonDeadViewModel _viewModel = new CommonDeadViewModel();

        private IDeadUiAdapter _adapter;

        

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }


        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
        }


        private void InitGui()
        {
            _viewModel.ButtonGroupShow = false;
            _viewModel.ContinueBtnClick = _adapter.BackToHall;
            _viewModel.ObserverBtnClick = _adapter.Observe;
        }

        public CommonDeadModel(IDeadUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        
        public override void Update(float interval)
        {
            UpdateButtonState();
        }

        private bool haveRegister;
        private void UpdateButtonState()
        {
            if (!_adapter.DeadButtonShow)
            {
                _viewModel.ButtonGroupShow = false;
            }
            else
            {
                _viewModel.ButtonGroupShow = true;
                _viewModel.ContinueBtnText = _adapter.HaveAliveTeammate ? BackToHallText : ContinueText;
            }
        }

        private void RegisterReceiver()
        {
            if (!haveRegister)
            {
                haveRegister = true;
                _adapter.SetCrossVisible(false);
                _viewModel.MaskGroupShow = true;
            }
            
        }

        private void UnRegisterReceiver()
        {
            if (haveRegister)
            {
                haveRegister = false;
                _adapter.SetCrossVisible(true);
                _viewModel.MaskGroupShow = false;
            }
        }

        public string BackToHallText
        {
            get { return "返回大厅"; }
        }

        public string ContinueText
        {
            get { return "继续"; }
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);
            if (!enable)
            {
                UnRegisterReceiver();
            }
            else
            {
                RegisterReceiver();
            }
           
        }
    }
}
