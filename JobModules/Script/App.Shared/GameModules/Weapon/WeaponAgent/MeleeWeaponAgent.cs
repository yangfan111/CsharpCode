
using System;
using Core;
using Core.EntityComponent;

namespace App.Shared.GameModules.Weapon
{
    [WeaponSpecies(EWeaponSlotType.MeleeWeapon)]
    public class MeleeWeaponAgent : CommonWeaponAgent
    {
        //public TacticSlotHandler(
        //    PlayerEntity playerEntity,
        //    ISharedPlayerWeaponGetter playerWeaponActionLogic,
        //    WeaponComponent weapon, 
        //    IReservedBulletLogic reservedBulletController, 
        //    IPlayerSoundManager playerSoundManager) : 
        //    base(playerEntity, playerWeaponActionLogic, weapon, reservedBulletController, playerSoundManager)
        //{
        //}

        //public override void OnExpend(System.Action<EWeaponSlotType> callback)

        //{

        //}
        public MeleeWeaponAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHelper grenadeHelper) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHelper)
        {
        }
    }
}
