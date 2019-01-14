using App.Shared.Components.Bag;
using App.Shared.Util;
using Core;
using Core.Bag;
using Core.GameModeLogic;
using Core.Sound;

namespace App.Shared.WeaponLogic
{
    [WeaponSpecies(EWeaponSlotType.MeleeWeapon)]
    public class MeleeSlotHandler : WeaponSlotBaseHandler
    {
        public MeleeSlotHandler(
            PlayerEntity playerEntity,
            IPlayerWeaponActionLogic playerWeaponActionLogic,
            WeaponComponent weapon, 
            IReservedBulletLogic reservedBulletController,
            IPlayerSoundManager playerSoundManager) : base(playerEntity, playerWeaponActionLogic, weapon, reservedBulletController, playerSoundManager)
        {
        }

        public override void OnCost(System.Action<EWeaponSlotType> onslotRestuff)
        {

        }
       
    }
}
