using Core;
using Core.Utils;

namespace App.Shared.GameMode
{


    /// <summary>
    /// Defines the <see cref="SurvivalModeLogicFactory" />
    /// </summary>
    public class GameSurvivalModeController : GameModeControllerBase
    {
        public override void Initialize(Contexts contexts, int modeId)
        {
            SetMode(modeId);
            ProcessListener = new ModeProcessListener();
            ReservedBulletHandler = new SharedReservedBulletHandler();
            PickupHandler = new SurvivalPickupDropHandler(contexts,modeId);
            SlotLibary = WeaponSlotsLibrary.Allocate(EWeaponSlotsGroupType.Default);
        }

        public override EWeaponSlotType GetSlotByIndex(int index)
        {
            if (index <= 0 || index > (int)EWeaponSlotType.Length)
            {
                return EWeaponSlotType.None;
            }
            return (EWeaponSlotType)index;
        }

        public override bool CanModeSwitchBag
        {
            get { return false; }
        }
        public override bool CanModeAutoReload
        {
            get { return false; }
        }
        //public override void RecoverPlayerWeapon(PlayerEntity player)
        //{
        //}
    }
}
