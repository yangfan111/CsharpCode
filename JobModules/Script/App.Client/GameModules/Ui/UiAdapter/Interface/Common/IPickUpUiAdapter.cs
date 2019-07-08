using App.Client.GameModules.Ui.Logic;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IPickUpUiAdapter : IAbstractUiAdapter
    {
        void RegisterKeyReceiver(IKeyReceiver receiver);
        void RegisterPointerReceiver(IPointerReceiver receiver);
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