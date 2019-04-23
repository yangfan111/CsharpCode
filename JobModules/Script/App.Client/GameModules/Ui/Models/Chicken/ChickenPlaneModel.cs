using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.ViewModels.Chicken;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;

namespace App.Client.GameModules.Ui.Models.Chicken
{

    public class ChickenPlaneModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenPlaneModel));
        private ChickenPlaneViewModel _viewModel = new ChickenPlaneViewModel();

        private IPlaneUiAdapter _adapter;

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



        public ChickenPlaneModel(IPlaneUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        public override void Update(float interval)
        {
            UpdatePlayerCount();
        }

        private void UpdatePlayerCount()
        {
            _viewModel.CurCount = _adapter.CurCount.ToString();
            _viewModel.TotalCount = _adapter.TotalCount.ToString();
        }
    }
}
