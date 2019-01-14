using Core.Attack;
using Core.Bag;
using Core.EntityComponent;
using Core.Free;
using Core.Statistics;
using Core.WeaponLogic.Throwing;
using Entitas;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace Core.WeaponLogic
{
    /// <summary>
    /// 跟着人走的属性
    /// </summary>
    public interface IPlayerWeaponState 
    {
        ////////////////////////////////////////////////////////////
        /// Provided
        ////////////////////////////////////////////////////////////
        Entity Owner { get; }
        EntityKey Key { get; }
        int ClientTime { get; }
        float ViewPitch { get; }
        float ViewYaw { get; }
        Vector3 BulletEmitPosition { get; }
        float HorizontalVeocity { get;  }
        bool IsAir { get; }
        bool IsDuckOrProneState { get; }
        bool IsAiming { get; } // 开镜或肩射
        bool IsProne { get; } // 爬行
        bool IsReload { get; }
        bool IsBolted { get; set; }
        bool IsFiring { get; }
        bool IsFireHold { get; }
        bool IsFireEnd { get; }
        int CurrentWeapon { get; }
        GameObject CurrentWeaponGo { get; }
        EBulletCaliber Caliber { get; }


        ////////////////////////////////////////////////////////////
        /// IKickbackLogic, IKickbackDecayLogic 后坐力
        ////////////////////////////////////////////////////////////
        float NegPunchPitch { get; set; }
        float NegPunchYaw { get; set; }
        float WeaponPunchPitch { get; set; }
        float WeaponPunchYaw{ get; set; }
        int PunchDecayCdTime { get; set; } // 开火后坐力效果时间, 在这个时间内，不回落
        bool PunchYawLeftSide { get; set; } // PunchYaw随机的方向


        ////////////////////////////////////////////////////////////
        /// IAccuracyLogic
        ////////////////////////////////////////////////////////////
        float LastAccuracy { get; set; }

        
        ////////////////////////////////////////////////////////////
        /// IFireReady
        ////////////////////////////////////////////////////////////
        int NextAttackTimer { get; set; }
        int LoadedBulletCount { get; set; }
        int SpecialReloadCount { get; }
        int BulletCountLimit { get;}
        int ReservedBulletCount { get; set; }
        int LastFireTime { get; set; }
        float ReloadSpeed { get; }
        bool IsAlwaysEmptyReload { get; }
        /// <summary>
        /// 开镜状态开枪后不立即拉栓，结束开镜状态才拉栓
        /// </summary>
        bool IsSniperFire { get; }

        ////////////////////////////////////////////////////////////
        /// ContinuesShoot
        ////////////////////////////////////////////////////////////
        int ContinuesShootCount { get; set; }
        bool ContinuesShootDecreaseNeeded { get; set; }
        int ContinuesShootDecreaseTimer { get; set; }
        int BurstShootCount { get; set; }
        Vector3 LastBulletDir { get; set; }
        float LastSpreadX { get; set; }
        float LastSpreadY { get; set; }
        EFireMode FireMode { get; set; }
        void ShowFireModeChangeTip(EFireMode newFireMode);
        void ShowFireModeUnchangeTip();
        void ShowNoBulletTip();
        bool IsPrevCmdFire { get; set; }
        bool CanFire();
        bool CanMeleeFire();
        bool CanCameraFocus(); 
        void OnDefaultFire(bool NeedActionDeal = false);
        void OnSpecialFire(bool NeedActionDeal = false);
        void EndSpecialFire();
        bool PullBolting { get; set; }
        bool ResumeGunSightAfterPullBolt { get; }
        void OnSwitchMode(CommonFireConfig common);
        void OnSwitchWeapon();
        void OnWeaponStateChanged();

        int LastGrenadeId { get; set; }

        void OnLightMeleeFire(System.Action callback);
        void OnSecondLightMeleeFire(System.Action callback);
        void OnMeleeSpecialFire(System.Action callback);
        int ContinuousAttackTime { get; set; }
        /// <summary>
        /// 如果没有攻击没有正常结束，需要有一个最大时间来重置攻击中的状态
        /// </summary>
        int NextAttackingTimeLimit { get; set; }
        bool MeleeAttacking { get; set; }
        bool RangeAttacking { get; set; }

        void BreakReloading();
        void ForceBreakReloading(System.Action callback);
        void StartMeleeAttack(int startTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
        void CancelMeleeAttack();

        //Throwing
        void ReadyThrowing();
        void StartThrowing();
        void SwitchThrowingMode(bool isNearThrow);
        void ForceStopThrowing();
        void OnWeaponCost();
        void UnmountWeaponByAction();
        ThrowingActionInfo ThrowingActionInfo { get; }
        bool IsThrowingStartFly { get; set; }
        void OnFireOnce();
        void OnGrenadeThrowingOnce();

        int PlaySoundOnce(int id);
        Transform SightLocatorOnPartP1 { get; }
        Transform SightLocatorOnWeaponP1 { get; }
        Vector3 CameraAngle { get; }
        Vector3 CameraPosition { get; }
        IFreeData FreeData { get; }
        
        Vector3? GetSightFireViewPosition { get; }
        Vector3? MuzzleP3LocatorPosition { get; }
    }
}