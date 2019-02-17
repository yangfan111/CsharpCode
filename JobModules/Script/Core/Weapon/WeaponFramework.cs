using Assets.XmlConfig;
using Core.EntityComponent;
using Core.Enums;
using System;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;

namespace Core
{
    public enum EWeaponSlotType
    {
        None = 0,
        PrimeWeapon,
        SecondaryWeapon,
        PistolWeapon,
        MeleeWeapon,
        ThrowingWeapon,
        TacticWeapon,
        Length,

        Pointer =99,
    }
    public partial class GameGlobalConst
    {
        public const int WeaponBagMaxCount = 2;
        public static readonly int WeaponSlotMaxLength = (int)EWeaponSlotType.Length;
        public const int WeaponEntityType = 11;

    }

    public enum EWeaponSlotsGroupType
    {
        Default,
        Group,
    }
    public delegate void WeaponSlotUpdateEvent(EntityKey key);
    public delegate void WeaponHeldUpdateEvent();
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class WeaponSpeciesAttribute : Attribute
    {
        public readonly EWeaponSlotType slotType;

        public WeaponSpeciesAttribute(EWeaponSlotType stype)
        {
            this.slotType = stype;
        }
    }
  
    public interface IPlayerWeaponComponentGetter
    {
    }
    public interface IPlayerWeaponControllerFrameWork
    { }

    public interface IPlayerModuleComponentAgent
    {
       
    }
    public interface IPlayerModuleController { }

    /// <summary>
    /// 武器相关的人物动作 地面-当前武器-背包
    /// </summary>
   
    
    public struct ItemInfo
    {
        public ECategory Category;
        public int ItemId;
        public int Count;
    }


}