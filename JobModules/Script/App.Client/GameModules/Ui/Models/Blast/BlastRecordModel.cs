using App.Client.GameModules.Ui.Models.Group;
using App.Client.GameModules.Ui.UiAdapter;
using Core.Enums;
using UIComponent.UI;

namespace App.Client.GameModules.Ui.Models.Blast
{
    public class BlastRecordModel : GroupRecordModel
    {
        public BlastRecordModel(IGroupRecordUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }
        private IGroupRecordUiAdapter _adapter;
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            var viewModel = base.ViewModel;
            var name = viewModel.ResourceAssetName;
            FindChildGo(name).name = "BlastRecord";
        }

        protected override void InitTitleInfo(UIList titleList, EUICampType campType)
        {
            titleList.Add<BlastRecordInfo>(new GroupRecordViewData() { IsTitle = true, NeedShow = true, CampType = campType});
        }

        protected override void InitAllRecordInfo(UIList campInfoList, EUICampType campType)
        {
            for (int i = 0; i < MaxInfoCount; i++)
            {
                campInfoList.Add<BlastRecordInfo>(new GroupRecordViewData() { IsTitle = false, NeedShow = false, Rank = i + 1, CampType = campType, CanShowC4 = _adapter.MyCamp == EUICampType.T});
            }
        }

        protected override void UpdateInfoShow(IUIItem iUIItem, bool show)
        {
            (iUIItem.Data as GroupRecordViewData).NeedShow = show;
            (iUIItem as BlastRecordInfo).UpdateInfoView();
        }

    }
}
