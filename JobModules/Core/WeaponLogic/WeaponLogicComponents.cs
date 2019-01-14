using UnityEngine;
using Core.WeaponLogic.Common;
using Core.EntityComponent;
using WeaponConfigNs;
using Core.WeaponLogic.Bullet;

namespace Core.WeaponLogic
{
    public interface IBeforeFireBullet
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之前调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet);
    }

    public interface IAfterFireBullet
    {
        /// <summary>
        /// 收到Fire命令，并在每发子弹之后调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet);    
    }

    public interface IAfterFireCmd
    {
        /// <summary>
        /// 收到Fire命令，在执行整个Cmd后调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void AfterFireCmd(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bulletCount);
    }


    public interface IIdle
    {
        /// <summary>
        /// 没有Fire命令
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd);
    }

    public interface IFrame
    {
        /// <summary>
        /// 每个Cmd调用
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd);
    }

    /// <summary>
    /// PunchYaw, PunchPitch, PunchYawDirection, RunUpMax, PunchDecayCD
    /// </summary>
    public interface IKickbackLogic : IFrame, IAfterFireBullet
    {
        void SetVisualConfig(ref VisualConfigGroup config);
    }

    /// <summary>
    /// 计算是否可以射击（子弹数，射击CD)
    /// NextAttackTimer, LoadedBulletCount, LastFireTime
    /// </summary>
    public interface IFireBulletModeLogic: IAfterFireCmd, IFrame
    {
        int GetFireTimes(IPlayerWeaponState playerWeapon);
        
    }

    /// <summary>
    /// 计算Accuracy
    /// </summary>
    public interface IAccuracyLogic : IIdle, IBeforeFireBullet
    {
        
    }

    /// <summary>
    /// 创建子弹对象
    /// </summary>
    public interface IBulletFactory
    {
        int BulletHitCount { get; }
        void CreateBullet(IPlayerWeaponState playerWeapon, 
            Vector3 direction, 
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher, 
            int cmdSeq, 
            int renderTime);
    }

    /// <summary>
    /// 创建投掷物对象
    /// </summary>
    public interface IThrowingFactory
    {
        float ThrowingInitSpeed(bool isNear);
        int BombCountdownTime { get; }
        ThrowingConfig ThrowingConfig { get; }
        EntityKey CreateThrowing(IPlayerWeaponState playerWeapon, Vector3 direction, int renderTime, float initVel);
        void UpdateThrowing(EntityKey entityKey, bool isThrow, float initVel);
        void DestroyThrowing(EntityKey entityKey);
    }

    /// <summary>
    /// 创建开火相关特效
    /// </summary>
    public interface IFireEffectFactory
    {
        void CreateBulletDropEffect(IPlayerWeaponState playerWeapon);
        void CreateSparkEffect(IPlayerWeaponState playerWeapon);
    }

    /// <summary>
    /// 计算Spread
    /// </summary>
    public interface ISpreadLogic: IBeforeFireBullet
    {
        
    }

    public interface IFireBulletCounter: IBeforeFireBullet, IIdle
    {
        
    }

    public interface IAutoFireLogic : IAfterFireBullet
    {
        bool Running { get; }
        void Reset();
    }

    public interface IMoveSpeedLogic
    {
        float GetMoveSpeedMs();
    }
}