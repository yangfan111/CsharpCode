using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.Utility;
using App.Shared;
using UnityEngine.EventSystems;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonFocusMaskModel : ClientAbstractModel, IUiSystem
    {
        private CommonFocusMaskViewModel _viewModel = new CommonFocusMaskViewModel();
        private IFocusMaskUIAdapter adapter = null;
        private bool _isSafeLocked = false;
        private bool _isLocked = false;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonFocusMaskModel(IFocusMaskUIAdapter adapter) : base(adapter)
        {
            this.adapter = adapter;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            BindEventTrigger();
            _isLocked = !adapter.IsShowCrossHair;
        }

        public override void Update(float interval)
        {
            if (SharedConfig.isFouces && _isSafeLocked)
            {
                SetCursorState(adapter.IsShowCrossHair);
                _viewModel.Show = false;
            }
            else
            {
                _isSafeLocked = false;
                SetCursorState(false);
                _viewModel.Show = true;
            }
        }
        private void SetCursorState(bool state)
        {
            if (_isLocked == state) return;
            _isLocked = state;
            if (_isLocked)
            {
                CursorLocker.LockCursor();
            }
            else
            {
                CursorLocker.UnLockCursor();
            }
        }

        private void BindEventTrigger()
        {
            _viewModel.OnMaskEventTriggerHoverListener = OnHover;
            _viewModel.OnMaskEventTriggerMouseDownListener = OnDown;
        }

        private void OnHover(BaseEventData baseEventData)
        {
            _isSafeLocked = true;
        }
        private void OnDown(BaseEventData baseEventData)
        {
            _isSafeLocked = true;
        }
    }
}    
