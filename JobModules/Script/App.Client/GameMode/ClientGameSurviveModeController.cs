using App.Shared.GameMode;
using Core;

namespace App.Client.GameMode
{
    /// <summary>
    /// Defines the <see cref="ClientGameSurviveModeController" />
    /// </summary>
    public class ClientGameSurviveModeController : GameSurvivalModeController
    {
        public override void Initialize(Contexts contexts, int modeId)
        {
            SetMode(modeId);
            ProcessListener = new ModeProcessListener();
            ReservedBulletHandler = new SharedReservedBulletHandler();
            PickupHandler = new ClientSurvivalPickupDropHandler(contexts);
            SlotLibary = WeaponSlotsLibrary.Allocate(EWeaponSlotsGroupType.Default);
        }
    }
}
