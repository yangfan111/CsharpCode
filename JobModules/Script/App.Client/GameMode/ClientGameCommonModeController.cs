using App.Shared.GameMode;
using Core;

namespace App.Client.GameMode
{
    /// <summary>
    /// Defines the <see cref="ClientGameCommonModeController" />
    /// </summary>
    public class ClientGameCommonModeController : GameCommonModeController
    {
        public override void Initialize(Contexts contexs, int modeId)
        {
            SetMode(modeId);
            serverInitHandler = new ServerWeaponInitHandler();
            ProcessListener = new CommonModeProcessListener();
            PickupHandler = new ClientCommonPickupDropHandler(contexs, ModeId);
            ReservedBulletHandler = new LocalReservedBulletHandler();
            SlotLibary = WeaponSlotsLibrary.Allocate(EWeaponSlotsGroupType.Group);
            
        }
       
        
        
        
        
    }
}
