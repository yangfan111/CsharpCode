using System;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;


namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="WeaponPartsRefreshStruct" />
    /// </summary>
    public struct WeaponPartsRefreshStruct
    {
        public WeaponScanStruct weaponInfo;

        public EWeaponSlotType slot;

        public WeaponPartsStruct oldParts;

        public WeaponPartsStruct newParts;

        public bool armInPackage;

        public EntityKey lastWeaponKey;

        public override string ToString()
        {
            return string.Format("scan:{0},oldParts:{1},newParts:{2}", weaponInfo, oldParts, newParts);
        }
    }
    

    public struct WeaponDrawAppearanceStruct
    {
        public float           holsterParam;
        public EWeaponSlotType targetSlot;
        public float           drawParam;
        public bool            armOnLeft;

        public float switchParam
        {
            get { return holsterParam * 10 + drawParam; }
        }
    }

    public struct WeaponSystemImplStruct
    {
        public PlayerEntity playerEntity;
        public PlayerWeaponController weaponController;
    }

 
    public enum EInOrOff
    {
        In,
        Off,
    }

    public enum EWeaponFireSpecies
    {
        None,
        Pistol,
        Rifle,
        Fixed,
    }

    public enum EWeaponSide
    {
        Left,
        Right, 
    }

    //public delegate void WeaponSlotUpdateEvent(EntityKey key);
//    public delegate void WeaponHeldUpdateEvent();
    public delegate void WeaponProcessEvent(IPlayerWeaponProcessor controller, EWeaponSlotType slot);

    public delegate void WeaponSwitchEvent(IPlayerWeaponProcessor controller, int weaponId, EInOrOff op);

    public delegate bool WeaponSystemCmdSpecificFilter(PlayerEntity     playerEntity, PlayerWeaponController controller,
                                                       IUserCmd         userCmd);
   
    public delegate void WeaponDropEvent(IPlayerWeaponProcessor controller, EWeaponSlotType slot,
                                         EntityKey              dropedWeapon);
}