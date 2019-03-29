using Core;
using Core.EntityComponent;
using System;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// 4 location : ground body hand pacakge
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.None)]

    public class NoneWeaponAgent : WeaponBaseAgent
    {
        public NoneWeaponAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHelper grenadeHelper) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHelper)
        {
        }

        public override bool ExpendWeapon()
        {
            return false;
        }

        public override int FindNextWeapon(bool autoStuff)
        {
            return -1;
        }

        public override void ReleaseWeapon()
        {
        }

        public override WeaponEntity ReplaceWeapon(EntityKey Owner, WeaponScanStruct orient, ref WeaponPartsRefreshStruct refreshParams)
        {
            throw new NotImplementedException();
        }
    }
}
