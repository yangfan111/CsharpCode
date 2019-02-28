using WeaponConfigNs;
using Core.Attack;
using Core.Utils;
using App.Shared.WeaponLogic;
using App.Shared;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Utils.Configuration;
using Utils.Singleton;
using App.Shared.Components.Weapon;
using System;
using Utils.Compare;

namespace Core.WeaponLogic.Common
{
    //public abstract class MeleeFireHanlder
    //{
    //    protected IWeaponCmd cmd;
    //    protected Weapon.
    //}
    //public class FirstHitHanlder:MeleeFireHanlder
    //{
    //    public void Assign(IWeaponCmd in_cmd)
    //    {
    //        cmd = in_cmd;
    //    }
    //    public bool Vertify(WeaponEntity enity)
    //    {
    //      WeaponRuntimeInfoComponent weaponState = enity.weaponRuntimeInfo;
    //    }
    //}

    public class MeleeFireLogic : IFireLogic
    {
        private static readonly LoggerAdapter Logger                     = new LoggerAdapter(typeof(MeleeFireLogic));
        private readonly MeleeAttackTimeController _attackTimeController = new MeleeAttackTimeController();
        private const int _maxCD                                         = 5000;
        private MeleeFireLogicConfig _config;
        private Contexts _contexts;

        public MeleeFireLogic(Contexts contexts, MeleeFireLogicConfig config)
        {
            _config = config;
            _contexts = contexts;
        }
   

        public void OnFrame(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponId = playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponId(_contexts);
           // _attackTimeController.TimeUpdate(cmd.FrameInterval * 0.001f, weaponId);
            var weaponState = weaponEntity.weaponRuntimeInfo;
           // if(!_attackTimeController.CanAttack) return;
            // if (playerEntity.time.ClientTime < weaponState.NextAttackTimePeriodStamp) return;
            var nowTime = playerEntity.time.ClientTime;
            var delta =  weaponState.NextAttackPeriodStamp-nowTime;
            if (delta >= 0)
                DebugUtil.LogInUnity("period delta:" + delta);
            delta = weaponState.ContinueAttackEndStamp - nowTime;
            if (delta >= 0)
                DebugUtil.LogInUnity("contineTime delta:" + delta);
           
            if (cmd.IsFire)
            {
                // 轻击1
                if (nowTime > weaponState.NextAttackPeriodStamp)
                {
                    playerEntity.stateInterface.State.LightMeleeAttackOne(OnAttackAniFinish);
                   // _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                    weaponState.NextAttackPeriodStamp       = nowTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                    weaponState.ContinueAttackStartStamp    = nowTime + _config.AttackOneCD;
                    weaponState.ContinueAttackEndStamp      = nowTime + _config.ContinousInterval;

                    DebugUtil.LogInUnity("First MeleeAttack", DebugUtil.DebugColor.Green);
                }
                 
                //    if (playerEntity.time.ClientTime > weaponState.ContinuousAttackTime)
                //{
                //    playerEntity.stateInterface.State.LightMeleeAttackOne(() => { _attackTimeController.FinishAttack();});

                //    _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                //    weaponState.NextAttackTimePeriodStamp                    = playerEntity.time.ClientTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                //    Logger.InfoFormat("MeleeAttackOne----------------");
                // 轻击2
                
                else if(CompareUtility.IsBetween(nowTime,weaponState.ContinueAttackStartStamp, weaponState.ContinueAttackEndStamp))
                {
                    weaponState.ContinueAttackStartStamp                       = 0;
                    weaponState.ContinueAttackEndStamp                         = 0;
                    weaponState.NextAttackPeriodStamp                          = Math.Max(nowTime + _config.AttackOneCD, weaponState.ContinueAttackEndStamp);
                    playerEntity.stateInterface.State.LightMeleeAttackTwo(OnAttackAniFinish);
                    // _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                    //weaponState.ContinuousAttackTime                         = playerEntity.time.ClientTime;
                    DebugUtil.LogInUnity("Second MeleeAttack", DebugUtil.DebugColor.Green);
                }
            }
            else if( cmd.IsSpecialFire && playerEntity.time.ClientTime >= weaponState.NextAttackPeriodStamp)
            {
                playerEntity.stateInterface.State.MeleeSpecialAttack(OnAttackAniFinish);
            //    _attackTimeController.SetMeleeInterprutTime(_config.SpecialAttackInterval);
                Logger.InfoFormat("MeleeAttackSpecial----------------");
                weaponState.NextAttackPeriodStamp                              = nowTime + _config.SpecialDamageInterval;
            }
           
            AfterAttack(playerEntity, cmd);
        }
        private void OnAttackAniFinish()
        {
            DebugUtil.LogInUnity("Action finish", DebugUtil.DebugColor.Green);
        }
        public void AfterAttack(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var weaponState = playerEntity.GetWeaponRunTimeInfo(_contexts);
       //     weaponState.NextAttackingTimeLimit = playerEntity.time.ClientTime + _maxCD;
            //TODO 声音和特效添加 
            if(cmd.IsFire)
            {
                StartMeleeAttack(playerEntity, cmd.RenderTime + _config.DamageInterval,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {
                StartMeleeAttack(playerEntity, cmd.RenderTime + _config.SpecialDamageInterval,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            playerEntity.GetController<PlayerWeaponController>().OnExpend(_contexts, 
                (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        private void StartMeleeAttack(PlayerEntity playerEntity, int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if (playerEntity.hasMeleeAttackInfoSync)
            {
                playerEntity.meleeAttackInfoSync.AttackTime = attackTime;
            }
            else
            {
                playerEntity.AddMeleeAttackInfoSync(attackTime);
            }
            if (playerEntity.hasMeleeAttackInfo)
            {
                playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                playerEntity.meleeAttackInfo.AttackConfig = config;
            }
            else
            {
                playerEntity.AddMeleeAttackInfo();
                playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                playerEntity.meleeAttackInfo.AttackConfig = config;
            }
        }
    }

    class MeleeAttackTimeController
    {
        //private float _attackTime;
        //private float _currentTime;
        //private int _oldWeaponId;
        //private int _currentWeaponId;

        //private bool _canAttack = true;
        //public bool CanAttack
        //{
        //    get { return _canAttack; }
        //    private set { _canAttack = value; }
        //}

        //public void TimeUpdate(float deltaTime, int? weaponId)
        //{
        //    CanAttack = _currentTime >= _attackTime;
        //    _currentTime += deltaTime;

        //    UpdateAttackCD(weaponId);
        //}

        //public void FinishAttack()
        //{
        //    CanAttack = true;
        //    _currentTime = 0;
        //    _attackTime = 0;
        //}

        //public void SetMeleeInterprutTime(int interval)
        //{
        //    _currentTime = 0;
        //    _attackTime = interval;
        //    CanAttack = true;

        //}

        //public void SetLightAttackTwoTime()
        //{
        //    _currentTime = 0;
        //    _attackTime = AttackTime.LightAttackTwoTime;
        //}

        //public void SetSpecialAttackTime()
        //{
        //    _currentTime = 0;
        //    _attackTime = AttackTime.SpecialAttackTime;
        //}

        //private void UpdateAttackCD(int? weaponId)
        //{
        //    if (null == weaponId) return;
        //    _currentWeaponId = weaponId.Value;

        //    var manager = SingletonManager.Get<MeleeAttackCDConfigManager>();
        //    if (_oldWeaponId != _currentWeaponId && manager.IsConfigExist(_currentWeaponId))
        //    {
        //        // AttackTime.LightAttackOneTime = manager.GetAttackOneCDById(_currentWeaponId);
        //        // AttackTime.LightAttackTwoTime = manager.GetAttackTwoCDById(_currentWeaponId);
        //    }
        //}

        /**
         * 根据武器，时间不同
         * 后续读表，策划配表
         */
        //private struct AttackTime
        //{
        //    public static float LightAttackOneTime = 0.3f;
        //    public static float LightAttackTwoTime = 0.3f;
        //    public static float SpecialAttackTime = 0.433f;
        //}
    }
}
