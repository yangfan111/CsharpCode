using App.Client.GameModules.Ui.Models.Common;
using App.Client.GameModules.Ui.UiAdapter;

namespace App.Client.GameModules.Ui.Models.Blast
{
    public class BlastWeaponHudModel : CommonWeaponHudModel
    {
        public BlastWeaponHudModel(IWeaponStateUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }
        private IWeaponStateUiAdapter _adapter;
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            var viewModel = base.ViewModel;
            var name = viewModel.ResourceAssetName;
            FindChildGo(name).name = "BlastWeaponHud";
        }
    }
}
