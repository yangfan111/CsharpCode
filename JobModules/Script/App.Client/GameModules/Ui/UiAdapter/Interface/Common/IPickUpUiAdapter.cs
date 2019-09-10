using App.Client.GameModules.Ui.Logic;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IPickUpUiAdapter : IAbstractUiAdapter
    {
        void RegisterKeyhandler(KeyHandler handler);
        void RegisterPointerhandler(PointerKeyHandler handler);
        SceneObjectCastLogic GetSceneObjectCastLogic();
        MapObjectCastLogic GetMapObjectCastLogic();
        VehicleCastLogic GetVehicleCastLogic();
        FreeObjectCastLogic GetFreeObjectCastLogic();
        DoorCastLogic GetDoorCastLogic();
        PlayerCastLogic GetPlayerCastLogic();
        PlayerStateTipLogic GetPlayerStateTipLogic();
        CommonCastLogic GetCommonCastLogic();
        BuffTipLogic GetBuffTipLogic();
        bool IsCountDown();
    }
}