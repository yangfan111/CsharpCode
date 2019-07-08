﻿using Core;
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
        public NoneWeaponAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHandler grenadeHandler) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHandler)
        {
        }

        public override bool ExpendWeapon(int reservedBullet)
        {
            return false;
        }

        public override int FindNextWeapon(bool autoStuff)
        {
            return -1;
        }
        internal override bool CanApplyPart
        {
            get { return false; }
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
