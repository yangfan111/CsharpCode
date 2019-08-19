
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
        internal override bool CanApplyPart
        {
            get { return false; }
        }
        //}
        public MeleeWeaponAgent(Func<EWeaponSlotType, EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHandler grenadeHandler) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHandler)
        {
        }
    }
}
