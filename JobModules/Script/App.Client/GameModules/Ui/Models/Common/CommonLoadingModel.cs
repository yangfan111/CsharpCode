using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using App.Client.GameModules.Ui.UiAdapter;


namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonLoadingModel : ClientAbstractModel, IUiSystem
    {
        ILoadingUiAdapter adapter;
        private bool isGameObjectCreated = false;

        private CommonLoadingViewModel _viewModel = new CommonLoadingViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonLoadingModel(ILoadingUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
        }
       public override void Update(float interval)
        {
            RefreshGui();
        }

        private void InitGui()
        {
            _viewModel.showActive = true;
        }

        private void RefreshGui()
        {
            if (adapter != null && isGameObjectCreated)
            {
                if (adapter != null)
                {
                    if (adapter.CurValue > 0 && adapter.CurValue <= 1)
                    {
                        _viewModel.showActive = true;
                        _viewModel.sliderValue = adapter.CurValue;
                        _viewModel.curText = adapter.CurText;
                    }                    
                    else
                        _viewModel.showActive = false;
                }
            }
        }
    }
}    
