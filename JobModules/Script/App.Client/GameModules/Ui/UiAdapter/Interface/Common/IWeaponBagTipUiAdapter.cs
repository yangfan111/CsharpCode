using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UserInputManager.Lib;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.Components.Ui.UiAdapter
{

    public interface IWeaponBagTipUiAdapter : IAbstractUiAdapter
    {
        PlayerEntity Player
        {
            get;
            set;
        }
    }
}
