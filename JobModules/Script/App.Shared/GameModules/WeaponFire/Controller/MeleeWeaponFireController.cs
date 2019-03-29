using Core.Attack;
using Core.Utils;
using System;
using Utils.Compare;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
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
    /// <summary>
    /// Defines the <see cref="MeleeWeaponFireController" />
    /// </summary>
    public class MeleeWeaponFireController : IWeaponFireController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeWeaponFireController));


        private const int _maxCD = 5000;

        private MeleeFireLogicConfig _config;

        public MeleeWeaponFireController(MeleeFireLogicConfig config)
        {
            _config = config;
        }

        public void OnUpdate(PlayerWeaponController controller, IWeaponCmd cmd)
        {

            var weaponId = controller.HeldWeaponAgent.ConfigId;
            // _attackTimeController.TimeUpdate(cmd.FrameInterval * 0.001f, weaponId);
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            // if(!_attackTimeController.CanAttack) return;
            // if (playerEntity.time.ClientTime < weaponState.NextAttackTimePeriodStamp) return;
            var nowTime = controller.RelatedTime;
            var delta = weaponState.NextAttackPeriodStamp - nowTime;
            delta = weaponState.ContinueAttackEndStamp - nowTime;


            if (cmd.IsFire)
            {
                // 轻击1
                if (nowTime > weaponState.NextAttackPeriodStamp)
                {
                    // _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                    weaponState.NextAttackPeriodStamp    = nowTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                    weaponState.ContinueAttackStartStamp = nowTime + _config.AttackOneCD;
                    weaponState.ContinueAttackEndStamp   = nowTime + _config.ContinousInterval;
                    controller.RelatedStateInterface.LightMeleeAttackOne(OnAttackAniFinish);
                    AfterAttack(controller, cmd);
                    //          DebugUtil.LogInUnity("First MeleeAttack", DebugUtil.DebugColor.Green);
                }

                //    if (playerEntity.time.ClientTime > weaponState.ContinuousAttackTime)
                //{
                //    playerEntity.stateInterface.State.LightMeleeAttackOne(() => { _attackTimeController.FinishAttack();});

                //    _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                //    weaponState.NextAttackTimePeriodStamp                    = playerEntity.time.ClientTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                //    Logger.InfoFormat("MeleeAttackOne----------------");
                // 轻击2

                else if (CompareUtility.IsBetween(nowTime, weaponState.ContinueAttackStartStamp, weaponState.ContinueAttackEndStamp))
                {

                    weaponState.ContinueAttackStartStamp = 0;
                    weaponState.ContinueAttackEndStamp   = 0;
                    weaponState.NextAttackPeriodStamp    = Math.Max(nowTime + _config.AttackOneCD, weaponState.ContinueAttackEndStamp);
                    controller.RelatedStateInterface.LightMeleeAttackTwo(OnAttackAniFinish);
                    AfterAttack(controller, cmd);
                    // _attackTimeController.SetMeleeInterprutTime(_config.AttackInterval);
                    //weaponState.ContinuousAttackTime                         = playerEntity.time.ClientTime;
                    //    DebugUtil.LogInUnity("Second MeleeAttack", DebugUtil.DebugColor.Green);
                }
               
            }
            else if (cmd.IsSpecialFire && nowTime >= weaponState.NextAttackPeriodStamp)
            {
                controller.RelatedStateInterface.MeleeSpecialAttack(OnAttackAniFinish);
                //    _attackTimeController.SetMeleeInterprutTime(_config.SpecialAttackInterval);
                Logger.InfoFormat("MeleeAttackSpecial----------------");
                weaponState.NextAttackPeriodStamp = nowTime + _config.SpecialDamageInterval;
                AfterAttack(controller, cmd);
            }
        }

        private void OnAttackAniFinish()
        {
        }

        public void AfterAttack(PlayerWeaponController controller, IWeaponCmd cmd)
        {
        
            //TODO 声音和特效添加 
            if (cmd.IsFire)
            {

              //  DebugUtil.MyLog("DamageInterval:"+_config.DamageInterval);
                StartMeleeAttack(controller, cmd.RenderTime + _config.DamageInterval,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {

                StartMeleeAttack(controller, cmd.RenderTime + _config.SpecialDamageInterval,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            controller.ExpendAfterAttack();
        }

        private void StartMeleeAttack(PlayerWeaponController controller, int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            controller.CreateSetMeleeAttackInfo(attackInfo, config);
            controller.CreateSetMeleeAttackInfoSync(attackTime);
        }
    }

    /// <summary>
    /// Defines the <see cref="MeleeAttackTimeController" />
    /// </summary>
    
}
