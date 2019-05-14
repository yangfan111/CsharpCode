using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using XmlConfig;

namespace WeaponConfigNs
{
    public class WeaponPartsAchive
    {
        public int UpperRail;
        public int LowerRail;
        public int SideRail;
        public int Magazine;
        public int Stock;
        public int Muzzle;
        public int Bore;
        public int Feed;
        public int Trigger;
        public int Interlock;
        public int Brake;

        public void CloneFrom(WeaponPartsAchive attach)
        {
            LowerRail = attach.LowerRail;
            UpperRail = attach.UpperRail;
            SideRail = attach.SideRail;
            Muzzle = attach.Muzzle;
            Magazine = attach.Magazine;
            Stock = attach.Stock;
            Bore = attach.Bore;
            Feed = attach.Feed;
            Trigger = attach.Trigger;
            Interlock = attach.Interlock;
            Brake = attach.Brake;
        }
        
    }

    /// <summary>
    /// 需要被配件修改的变量，统一用Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ChangeByPartAttribute : Attribute
    {
        public ChangeByPartAttribute(WeaponAttributeType attributeType, PartModifyType modifyType)
        {
            AttributeType = attributeType;
            ModifyType = modifyType;
        }
        public WeaponAttributeType AttributeType;
        public PartModifyType ModifyType;
    }

    [XmlType("Weapon")]
    public class WeaponResConfigItem : ItemBaseConfig
    {
        public int[] Parts;
        public int AvatorId;
        public int Caliber;
        public int[] ActionDeal;
        public int[] SpecialType;
        public float AimModelScale;
        public float AimFov;
        public float ShiftFov;
        public int PickSound;
        public float Weight;
        public int Sort;
        public int Workshop; //厂牌
        public List<int> ApplyParts;  //可解锁与装配配件

        public bool IsSnipperType
        {
            get
            {
                return SpecialType != null && SpecialType.Contains((int)ESpecialWeaponType.SniperFrie);
            }
        }
    }

    public enum ActionDealEnum
    {
        Null,
        Fire,               //1
        Reload,             //2
        ReloadEmpty,        //3
        SwitchWeapon,       //4
    }

    public enum ESpecialWeaponType
    {
        ReloadEmptyAlways = 1,
        SniperFrie = 2,
    }

    [XmlRoot("root")]
    public class NewWeaponConfig
    {
        public WeaponResConfigItem[] Items;
    }
    
    /// <summary>
    /// 所有的单位：
    /// 速度：m/s
    /// 时间：ms
    /// </summary>
    [XmlRoot]
    public sealed class WeaponConfigs 
    {
        public WeaponConfig[] Weapons;
        public static WeaponConfigs Load(string configStr)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(WeaponConfigs));
            return  (WeaponConfigs)writer.Deserialize(new StringReader(configStr));
            
        }
    }

    public enum EEnvironmentType
    {
        Wood,
        Concrete,
        Steel,
        Stone,
        Soil,
        Length,
    }

    public class EnvironmentDamageDecay
    {
        [XmlAttribute()] public EEnvironmentType Type;
        [XmlAttribute()] public float DecayFactor;
    }

    public sealed class WeaponConfig
    {
        [XmlAttribute()] public int Id { get; set; }
        [XmlAttribute()] public string Name { get; set; }
        [XmlElement("WeaponLogic",IsNullable = true)] public WeaponAbstractBehavior WeaponBehavior { get; set; }
        public List<AnimatorStateItem> AnimatorStateTimes { get; set; }
        public int RigidityDuration;
        public int RigidityEffect;
    }

    [XmlType("item")]
    public class AnimatorStateItem
    {
        public string StateName;
        public float StateTime;
    }

    [XmlInclude(typeof(DefaultWeaponSoundConfig))]
    [XmlInclude(typeof(MeleeWeaponSoundConfig))]
    public abstract class WeaponSoundConfig
    {
    }

    public class DefaultWeaponSoundConfig : WeaponSoundConfig
    {
        public int SwitchIn;
        [ChangeByPart(WeaponAttributeType.FireSound, PartModifyType.Replace)]
        public int Fire { get; set; }
        public int ReloadStart;
        public int ReloadEnd;
        public int PullBolt;
        public int OnShoulder;
        public int SwitchFireMode;
        public int ClipDrop;
    }

    [XmlInclude(typeof(DefaultWeaponEffectConfig))]
    [XmlInclude(typeof(MeleeWeaponEffectConfig))]
    public abstract class WeaponEffectConfig
    {
    }

    public class DefaultWeaponEffectConfig : WeaponEffectConfig
    {
        [XmlElement(IsNullable = false, ElementName = "Spark")]
        [ChangeByPart(WeaponAttributeType.Spark, PartModifyType.Replace)]
        public int Spark { get; set; }
        [XmlElement(IsNullable = false, ElementName = "BulletDrop")]
        public int BulletDrop;
        [XmlElement(IsNullable = false, ElementName = "ClipDrop")]
        public int ClipDrop;
    }

    public class MeleeWeaponEffectConfig : WeaponEffectConfig
    {
    }

    public class MeleeWeaponSoundConfig : WeaponSoundConfig
    {
        [XmlElement(IsNullable = false, ElementName = "Left1")]
        public int Left1;
        [XmlElement(IsNullable = false, ElementName = "Left2")]
        public int Left2;
        [XmlElement(IsNullable = false, ElementName = "Right")]
        public int Right;
        [XmlElement(IsNullable = false, ElementName = "Transform")]
        public int Transform;
    }

    public class AssetInfo
    {
        [XmlAttribute()] public string BundleName;
        [XmlAttribute()] public string AssetName;
    }

    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    [XmlInclude(typeof(DoubleWeaponBehaviorConfig))]
    [XmlInclude(typeof(DefaultWeaponBehaviorConfig))]
    [XmlInclude(typeof(TacticWeaponBehaviorConfig))]
    public abstract class WeaponAbstractBehavior
    {
        
        [System.Obsolete]
        [XmlElement(IsNullable = true, ElementName = "Sound")]
        public WeaponSoundConfig SoundConfig { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Effect")]
        public WeaponEffectConfig EffectConfig { get; set; }

        public int[] AttachmentConfig { get; set; }

        [XmlAttribute()]
        public float MaxSpeed { get; set; }
        [XmlAttribute()]
        public bool CantRun { get; set; }
    }

    [XmlType("DoubleWeaponLogic")]
    public class DoubleWeaponBehaviorConfig  : WeaponAbstractBehavior
    {
        [XmlElement(IsNullable = true, ElementName = "LeftFire")]
        public DefaultWeaponAbstractFireFireLogicConfig LeftFireLogic { get; set; }
        [XmlElement(IsNullable = true, ElementName = "RightFire")]
        public DefaultWeaponAbstractFireFireLogicConfig RightFireLogic { get; set; }
    }

    [XmlType("DefaultWeaponLogic")]
    public class DefaultWeaponBehaviorConfig : WeaponAbstractBehavior
    {
        [XmlElement(IsNullable = true, ElementName = "Fire")]
        public DefaultWeaponAbstractFireFireLogicConfig FireLogic { get; set; }
    }

    [XmlType("TacticWeaponLogic")]
    public class TacticWeaponBehaviorConfig : WeaponAbstractBehavior
    {

    }

    [XmlType("Common")]
    public class CommonFireConfig
    {
        [XmlAttribute()] public int AttackInterval { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.Bullet, PartModifyType.Add)]
        public int MagazineCapacity { get; set; }
        [XmlAttribute()] public int SpecialReloadCount { get; set; }
        [XmlAttribute()] [DefaultValue("")] public string Tag { get; set; }
    }

    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    [XmlInclude(typeof(DefaultFireLogicConfig))]
    [XmlInclude(typeof(MeleeFireLogicConfig))]
    [XmlInclude(typeof(ThrowingFireLogicConfig))]
    public abstract class DefaultWeaponAbstractFireFireLogicConfig {
    }

    [XmlType("DefaultFireLogic")]
    public class DefaultFireLogicConfig : DefaultWeaponAbstractFireFireLogicConfig
    {
        [ChangeByPart(WeaponAttributeType.Fov, PartModifyType.Replace)]
        public float Fov { get; set; }
        [ChangeByPart(WeaponAttributeType.FocusSpeed, PartModifyType.Scale)]
        public float FocusSpeed { get; set; }
        [ChangeByPart(WeaponAttributeType.ReloadSpeed, PartModifyType.Scale)]
        public float ReloadSpeed { get; set; }
        [ChangeByPart(WeaponAttributeType.Breath, PartModifyType.Scale)]
        public float BreathFactor { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Basic")]
        public CommonFireConfig Basic { get; set; }

        [XmlElement(IsNullable = true, ElementName = "FireMode")]
        public FireModeLogicConfig FireModeLogic { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Bullet")]
        public BulletConfig Bullet { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Accuracy")]
        public AccuracyLogicConfig AccuracyLogic { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Spread")]
        public SpreadLogicConfig SpreadLogic { get; set; }

        [XmlElement(IsNullable = true, ElementName = "FireCounter")]
        public FireCounterConfig FireCounter { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Kickback")]
        public ShakeConfig Shake { get; set; }
    }

    [XmlType("MeleeFireLogic")]
    public class MeleeFireLogicConfig : DefaultWeaponAbstractFireFireLogicConfig
    {
        [XmlAttribute()] public int DamageInterval { get; set; }
        [XmlAttribute()] public int SpecialDamageInterval { get; set; }
        [XmlAttribute()] public int ContinousInterval { get; set; }
        [XmlAttribute()] public int AttackTotalInterval { get; set; }
        [XmlAttribute()] public int SpecialAttackTotalInterval { get; set; }
        [XmlAttribute()] public int AttackOneCD { get; set; }
        [XmlElement] public int LeftDamage { get; set; }
        [XmlElement] public int RightDamage { get; set; }
        [XmlElement] public float Range { get; set; }
        [XmlElement] public float Width { get; set; }
        [XmlElement] public float Height { get; set; }
        [XmlElement] public BodyDamageConfig[] DamageFactor { get; set; }
    }

    [XmlType("ThrowingFireLogic")]
    public class ThrowingFireLogicConfig : DefaultWeaponAbstractFireFireLogicConfig
    {
        [XmlElement(IsNullable = true, ElementName = "Basic")]
        public CommonFireConfig Basic { get; set; }

        [XmlElement(IsNullable = true, ElementName = "Throwing")]
        public ThrowingConfig Throwing { get; set; }
    }

    #region AccuracyConfig
    [XmlInclude(typeof(RifleAccuracyLogicConfig))]
    [XmlInclude(typeof(PistolAccuracyLogicConfig))]
    [XmlInclude(typeof(ShotgunAccuracyLogicConfig))]
    [XmlInclude(typeof(SniperAccuracyLogicConfig))]
    public abstract class AccuracyLogicConfig
    {
    }

    [XmlType("SniperAccuracy")]
    public class SniperAccuracyLogicConfig : BaseAccuracyLogicConfig
    {

    }

    [XmlType("ShotgunAccuracy")]
    public class ShotgunAccuracyLogicConfig : BaseAccuracyLogicConfig
    {

    }

    [XmlType("RifleAccuracy")]
    public class RifleAccuracyLogicConfig : BaseAccuracyLogicConfig
    {

    }

    public class BaseAccuracyLogicConfig : AccuracyLogicConfig
    {
        [XmlAttribute()] public float MaxInaccuracy { get; set; }
        [XmlAttribute()] public float AccuracyOffset { get; set; }
        [XmlAttribute()] public int AccuracyDivisor { get; set; }
    }

    [XmlType("PistolAccuracy")]
    public class PistolAccuracyLogicConfig : AccuracyLogicConfig
    {
        [XmlAttribute()] public float InitAccuracy { get; set; }
        [XmlAttribute()] public float MaxAccuracy { get; set; }
        [XmlAttribute()] public float MinAccuracy { get; set; }
        [XmlAttribute()] public float AccuracyFactor { get; set; }
    }
    #endregion

    #region SpreadConfig
    [XmlInclude(typeof(PistolSpreadLogicConfig))]
    [XmlInclude(typeof(SniperSpreadLogicConfig))]
    [XmlInclude(typeof(RifleSpreadLogicConfig))]
    [XmlInclude(typeof(ShotgunSpreadLogicConfig))]
    public abstract class SpreadLogicConfig
    {
    }

    public class FixedSpreadLogicConfig : SpreadLogicConfig
    {
        [XmlAttribute()] public float Value { get; set; }
        [XmlElement(IsNullable = true, ElementName = "Scale")]
        [ChangeByPart(WeaponAttributeType.Spread, PartModifyType.Scale)]
        public SpreadScale SpreadScale { get; set; }
    }

    [XmlType("ShotgunSpread")]
    public class ShotgunSpreadLogicConfig : FixedSpreadLogicConfig 
    {
        
    } 

    [XmlType("PistolSpread")]
    public class PistolSpreadLogicConfig : SpreadLogicConfig
    {
        [XmlAttribute()] public float AirParam { get; set; }
        [XmlAttribute()] public float LengthGreater13Param { get; set; }
        [XmlAttribute()] public float DuckParam { get; set; }
        [XmlAttribute()] public float DefaultParam { get; set; }
        [XmlElement(IsNullable = true, ElementName = "Scale")]
        public SpreadScale SpreadScale { get; set; }
    }

    [XmlType("SniperSpread")]
    public class SniperSpreadLogicConfig : SpreadLogicConfig
    {
        [XmlAttribute()] public float AirParam { get; set; }
        [XmlAttribute()] public float LengthParam1 { get; set; }
        [XmlAttribute()] public float LengthParam2 { get; set; }
        [XmlAttribute()] public float DuckParam { get; set; }
        [XmlAttribute()] public float DefaultParam { get; set; }
        [XmlAttribute()] public float FovAddParam { get; set; }
        [XmlElement(IsNullable = true, ElementName = "Scale")]
        public SpreadScale SpreadScale { get; set; }
    }

    [XmlType("RifleSpread")]
    public class RifleSpreadLogicConfig : SpreadLogicConfig
    {
        public Spread Default { get; set; }
        public Spread Aiming { get; set; }
        [XmlAttribute()] public float FastMoveSpeed { get; set; }
        [XmlElement(IsNullable = true, ElementName = "Scale")]
        [ChangeByPart(WeaponAttributeType.Spread, PartModifyType.Scale)]
        public SpreadScale SpreadScale { get; set; }
    }

    public class Spread
    {
        [XmlAttribute()] public float Base { get; set; }
        [XmlAttribute()] public float Duck { get; set; }
        [XmlAttribute()] public float Air { get; set; }
        [XmlAttribute()] public float FastMove { get; set; }
        [XmlAttribute()] public float Prone { get; set; }

        public static Spread operator *(Spread spread, float percent)
        {
            spread.Base *= percent;
            spread.Duck *= percent;
            spread.Air *= percent;
            spread.FastMove *= percent;
            spread.Prone *= percent;
            return spread;
        }
    }


    [XmlType("SpreadScale")]
    public class SpreadScale 
    {
        [XmlAttribute()] public float ScaleX { get; set; }
        [XmlAttribute()] public float ScaleY { get; set; }
    }

    #endregion

    #region ShakeConfig
    [XmlInclude(typeof(DefaultKickbackDecayLogicConfig))]
    public abstract class KickbackDecayLogicConfig
    {
    }

    [XmlType("DefaultKickbackDecay")]
    public class DefaultKickbackDecayLogicConfig : KickbackDecayLogicConfig
    {

    }
    [XmlInclude(typeof(RifleShakeConfig))]
    [XmlInclude(typeof(PistolShakeConfig))]
    [XmlInclude(typeof(ShotgunShakeConfig))]
    [XmlInclude(typeof(SniperShakeConfig))]
    public abstract class ShakeConfig
    {
    }

    [XmlType("RifleKickback")]
    public class RifleShakeConfig : ShakeConfig
    {
        public float DecaytimeFactor;
        public float FixedDecayFactor;
        public float LenDecayFactor;
        [XmlAttribute()] public float FastMoveSpeed { get; set; }
        public ShakeGroup Default { get; set; }
        public ShakeGroup Aiming { get; set; }
    }

    public class FixedShakeConfig : ShakeConfig
    {
        public float PunchPitch { get; set; }
        public float Decaytime;
        public float FixedDecayFactor;
        public float LenDecayFactor;
        public float FallbackOffsetFactor;
        public float PunchOffsetFactor;
    }
   
    [XmlType("SniperKickback")]
    public class SniperShakeConfig : FixedShakeConfig
    {

    }

    [XmlType("PistolKickback")]
    public class PistolShakeConfig : FixedShakeConfig
    {
        
    }

    [XmlType("ShotgunKickback")]
    public class ShotgunShakeConfig : FixedShakeConfig
    {

    }


    public class ShakeGroup
    {
        /// <summary>
        /// 角色各个状态下的震动参数
        /// </summary>
        [XmlElement(IsNullable = true)] public ShakeInfo Base { get; set; }
        [XmlElement(IsNullable = true)] public ShakeInfo Duck { get; set; }
        [XmlElement(IsNullable = true)] public ShakeInfo Air { get; set; }
        [XmlElement(IsNullable = true)] public ShakeInfo FastMove { get; set; }
        [XmlElement(IsNullable = true)] public ShakeInfo Prone { get; set; }
        [XmlAttribute] public float HPunchOffsetFactor;
        [XmlAttribute] public float VPunchOffsetFactor;
        [XmlAttribute] public float WeaponFallbackFactor;
    }

    public class ShakeInfo
    {
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.UpBase, PartModifyType.Scale)]
        public float UpBase { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.UpModifier, PartModifyType.Scale)]
        public float UpModifier { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.UpMax, PartModifyType.Scale)]
        public float UpMax { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.LateralBase, PartModifyType.Scale)]
        public float LateralBase { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.LateralModifier, PartModifyType.Scale)]
        public float LateralModifier { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.LateralMax, PartModifyType.Scale)]
        public float LateralMax { get; set; }
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.LateralTurnback, PartModifyType.Scale)]
        public float LateralTurnback { get; set; }
        
        
        public static  explicit operator  ShakeInfoStruct(ShakeInfo orient)
        {
            var newStruct = new ShakeInfoStruct();
            newStruct.UpMax = orient.UpMax;
            newStruct.UpBase = orient.UpBase;
            newStruct.UpModifier = orient.UpModifier;
            newStruct.LateralBase = orient.LateralBase;
            newStruct.LateralModifier = orient.LateralModifier;
            newStruct.LateralTurnback = orient.LateralTurnback;
            newStruct.LateralMax = orient.LateralMax;
            return newStruct;

        }
        
        
    }
    public struct ShakeInfoStruct
    {
        public float UpBase { get; set; }
        public float UpModifier { get; set; }
        public float UpMax { get; set; }
        public float LateralBase { get; set; }
        public float LateralModifier { get; set; }
        public float LateralMax { get; set; }
        public float LateralTurnback { get; set; }

      
        
    }
    #endregion

    [XmlInclude(typeof(DefaultFireModeLogicConfig))]
    public abstract class FireModeLogicConfig
    {
    }

    [XmlType("DefaultFireMode")]
    public class DefaultFireModeLogicConfig : FireModeLogicConfig
    {
        [XmlAttribute()][DefaultValue(1)] public int BurstCount { get; set; }
        [XmlAttribute()] [DefaultValue(1)] public int BurstAttackInnerInterval { get; set; }
        [XmlAttribute()] [DefaultValue(1)] public int BurstAttackInterval { get; set; }
        public EFireMode[] AvaliableModes { get; set; }
    }

    [XmlType("FireMode")]
    public enum EFireMode
    {
        Manual = 1,//单发
        Auto,//自动
        Burst //半自动？
    }
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    public class BulletConfig
    {
        [XmlElement(IsNullable = true, ElementName = "BodyDamage")] public BodyDamageConfig[] BodyDamages;
        [XmlAttribute(), ChangeByPart(WeaponAttributeType.Speed, PartModifyType.Scale)] public float EmitVelocity { get; set; }
        [XmlAttribute()] public float VelocityDecay;
        [XmlAttribute()] public float Gravity;
        [XmlAttribute()] public float MaxDistance;
        [XmlAttribute()] public float DistanceDecayFactor;
        [XmlAttribute()] public float PenetrableThickness;
        [XmlAttribute()] public int PenetrableLayerCount;
        [XmlAttribute()] public float BaseDamage;
        [XmlAttribute()] public int HitCount;
    }

    // ReSharper disable InconsistentNaming
    public enum EBulletCaliber
    {
        None,
        E556mm = 1,
        E762mm,
        E9mm,
        E45apc,
        E12No,
        E300Mag,
        E50AE,
        E57mm,
        Length,
    }
    // ReSharper restore InconsistentNaming

    [XmlType("BodyDamage")]
    public class BodyDamageConfig
    {
        [XmlAttribute()] public EBodyPart BodyPart;
        [XmlAttribute()] public float Factor;
    }

    public enum EBodyPart
    {
        None,
        Head,
        Neck,
        Chest,
        Stomach,
        Pelvis,
        UpperArm,
        ForeArm,
        Hand,
        Thigh,
        Calf,
        Foot,
        Length
    }

    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////
    [XmlInclude(typeof(RifleFireCounterConfig))]
    public abstract class FireCounterConfig
    {
    }

    [XmlType("RifleFireCounter")]
    public class RifleFireCounterConfig : FireCounterConfig
    {
        [XmlAttribute()] public int MaxCount { get; set; }
        [XmlAttribute()] public int DecreaseInitInterval { get; set; }
        [XmlAttribute()] public int DecreaseStepInterval { get; set; }
    }

    ///////////////////////////////////////////////////////
    public class ThrowingConfig 
    {
        [XmlElement(IsNullable = true, ElementName = "BodyDamage")] public BodyDamageConfig[] BodyDamages;

        //远投初速度
        [XmlAttribute()] public float FarInitSpeed;
        //抛投初速度
        [XmlAttribute()] public float NearInitSpeed;
        //最大伤害半径
        [XmlAttribute()] public float DamageRadius;
        //飞行速度衰减
        [XmlAttribute()] public float VelocityDecay;
        //碰撞速度衰减
        [XmlAttribute()] public float CollisionVelocityDecay;
        //重力
        [XmlAttribute()] public float Gravity;
        //爆炸倒计时间
        [XmlAttribute()] public int CountdownTime;
        //爆炸最大伤害
        [XmlAttribute()] public float BaseDamage;
        //入水特效ID
        [XmlAttribute()] public int EnterWaterEffectId;
        //爆炸特效ID
        [XmlAttribute()] public int BombEffectId;
        //水里爆炸特效ID
        [XmlAttribute()] public int WaterBombEffectId;
        //爆炸声音ID
        [XmlAttribute()] public int SoundId;
    }

}
