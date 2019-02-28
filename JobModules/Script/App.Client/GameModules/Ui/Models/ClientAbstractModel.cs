using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.Ui;

namespace App.Client.GameModules.Ui.Models
{
    public class ClientAbstractModel : AbstractModel, IUiGroupController
    {
        private IUiAdapter _adapter;
        private bool _uiState = true;
        private bool _canvasEnabled = true;

        private bool _canvasForceUpdate = true;

        public ClientAbstractModel(IUiAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            ViewModel.Visible(false);
        }

        public void OnUiRender(float interval)
        {
            SetCanvasEnabled(GetCanvasEnabled());
            if (_adapter.IsReady() == false || !GetCanvasEnabled() || !isVisible || !_viewInitialized) return;
            Update(interval);
        }

        public bool Enable
        {
            get { return _adapter.Enable; }
            set { _adapter.Enable = value; }
        }

        public void SetUiState(bool isShow)
        {
            _uiState = isShow;
            SetCanvasEnabled(GetCanvasEnabled());
        }

        protected void SetCanvasEnabled(bool enable)
        {
            if (!_viewInitialized) return;
            if (!_canvasForceUpdate && _canvasEnabled.Equals(enable)) return;
            _canvasForceUpdate = false;
            _canvasEnabled = enable;
            if (ViewModel != null) SetViewModelVisible();
            OnCanvasEnabledUpdate(enable);
        }

        protected virtual void OnCanvasEnabledUpdate(bool enable)
        {

        }
        public new void SetVisible(bool b)
        {
            isVisible = b;                     //设置标志位
            SetViewModelVisible();      //设置GameObject
            VisibleCallBack();
        }

        private void SetViewModelVisible()
        {
            ViewModel.Visible(!(!isVisible || !_canvasEnabled));
        }
        protected bool GetCanvasEnabled()
        {
            return _adapter.Enable && _uiState;
        }

        public virtual void Update(float interval)
        {

        }
    }
}