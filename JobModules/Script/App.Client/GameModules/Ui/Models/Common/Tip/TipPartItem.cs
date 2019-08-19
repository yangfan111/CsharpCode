using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using UIComponent.UI;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common.Tip
{
    public class TipPartItem : UIItem
    {
        private TipPartItemViewModel viewModel = new TipPartItemViewModel();

        protected override IUiViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

       // PartPosData partPosData;

        protected override void SetView()
        {
            var partPosData = Data as TipPartUiData;
            viewModel.PartName = TipConst.GetWeaponPartTypeName((EWeaponPartType)partPosData.Type);
            viewModel.PartNameColor = UiCommonColor.GetQualityColor(partPosData.Quality);
            var assetIconInfo = AssetBundleConstant.GetPartsQualityAssetInfo(partPosData.Quality);
            Loader.RetriveSpriteAsync(assetIconInfo.BundleName, assetIconInfo.AssetName, (sprite) => viewModel.PartQualitySprite = sprite);

            if (!partPosData.HaveWeaponPart())
            {
                viewModel.PartsText = "未装备";
                viewModel.PartsColor = UiCommonColor.GetQualityColor(0);
            }
            else
            {
                viewModel.PartsText = partPosData.Name;
                viewModel.PartsColor = UiCommonColor.GetQualityColor(partPosData.Quality);
            }
        }
    }
}
