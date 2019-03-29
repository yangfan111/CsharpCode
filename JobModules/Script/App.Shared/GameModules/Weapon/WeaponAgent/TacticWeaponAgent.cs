using System;
using Core;
using Core.EntityComponent;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="TacticWeaponAgent" />
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.TacticWeapon)]
    public class TacticWeaponAgent : CommonWeaponAgent
    {
        public TacticWeaponAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHelper grenadeHelper) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHelper)
        {
        }
    }
}
