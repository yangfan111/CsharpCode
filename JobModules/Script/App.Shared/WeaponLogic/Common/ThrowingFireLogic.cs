using Core.Utils;
using WeaponConfigNs;
using XmlConfig;
using Core.WeaponLogic.WeaponLogicInterface;
using App.Shared.WeaponLogic;
using App.Shared.GameModules.Weapon;
using App.Shared;

namespace Core.WeaponLogic.Common
{
    public class ThrowingFireLogic : IFireLogic
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingFireLogic));

        private IThrowingFactory _throwingFactory;

        private bool _initialized;

        private static int _switchCdTime = 300;
        private ThrowingFireLogicConfig _config;
        private Contexts _contexts;

        public ThrowingFireLogic(
            Contexts contexts,
            NewWeaponConfigItem newWeaponConfig,
            ThrowingFireLogicConfig config,
            IWeaponLogicComponentsFactory componentsFactory)
        {
            _config = config;
            _throwingFactory = componentsFactory.CreateThrowingFactory(newWeaponConfig, config.Throwing);
            _contexts = contexts;
        }

        public void OnFrame(PlayerEntity playerWeapon, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            if (!_initialized)
            {
                _initialized = true;
            }

            if (cmd.IsFire && cmd.FilteredInput.IsInput(EPlayerInput.IsLeftAttack))
            {
                DoReady(playerWeapon, cmd);
            }

            if (cmd.SwitchThrowMode)
            {
                DoSwitchMode(playerWeapon, cmd);
            }

            if (cmd.IsReload && cmd.FilteredInput.IsInput(EPlayerInput.IsReload))
            {
                DoPull(playerWeapon, cmd);
            }

            if (cmd.IsThrowing && cmd.FilteredInput.IsInput(EPlayerInput.IsThrowing))
            {
                DoThrowing(playerWeapon, cmd);
            }

            //判断打断
            CheckBrokeThrow(playerWeapon, cmd);
        }

        //准备
        private void DoReady(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var actioninfo = playerEntity.throwingAction.ActionInfo;
            if (!actioninfo.IsReady)
            {
                actioninfo.IsReady = true;
                actioninfo.ReadyTime = playerEntity.time.ClientTime;
                actioninfo.Config = _throwingFactory.ThrowingConfig;
                //准备动作
                playerEntity.stateInterface.State.InterruptAction();
                playerEntity.stateInterface.State.StartFarGrenadeThrow();
            }
        }

        //投掷/抛投切换
        private void DoSwitchMode(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var actioninfo = playerEntity.throwingAction.ActionInfo;
            if (actioninfo.IsReady && !actioninfo.IsThrow)
            {
                if ((playerEntity.time.ClientTime - actioninfo.LastSwitchTime) < _switchCdTime)
                {
                    return;
                }
                actioninfo.LastSwitchTime = playerEntity.time.ClientTime;
                actioninfo.IsNearThrow = !actioninfo.IsNearThrow;
                SwitchThrowingMode(playerEntity, actioninfo.IsNearThrow);
            }
        }
        
        private void SwitchThrowingMode(PlayerEntity playerEntity, bool isNearThrow)
        {
            if (isNearThrow)
            {
                playerEntity.stateInterface.State.ChangeThrowDistance(0);
            }
            else
            {
                playerEntity.stateInterface.State.ChangeThrowDistance(1);
            }
        }

        //拉栓
        private void DoPull(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var actionInfo = playerEntity.throwingAction.ActionInfo;
            if (actionInfo.IsReady && !actionInfo.IsPull)
            {
                actionInfo.IsPull = true;
                actionInfo.LastPullTime = playerEntity.time.ClientTime;
                actionInfo.ShowCountdownUI = true;
                actionInfo.IsInterrupt = false;
                //生成Entity
                int renderTime = cmd.RenderTime;
                var dir = BulletDirUtility.GetThrowingDir(playerEntity);
                actionInfo.ThrowingEntityKey = _throwingFactory.CreateThrowing(playerEntity, dir, renderTime, GetInitVel(playerEntity));
                playerEntity.GetCurrentWeaponData(_contexts).LastBulletDir = dir;
                //弹片特效
                if (cmd.IsReload)
                {
                    playerEntity.weaponEffect.PlayList.Add(EClientEffectType.PullBolt);
                }
                playerEntity.weaponState.BagLocked = true;
            }
        }

        //投掷
        private void DoThrowing(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var actionInfo = playerEntity.throwingAction.ActionInfo;
            if (actionInfo.IsReady && !actionInfo.IsThrow)
            {
                if (!actionInfo.IsPull)
                {
                    DoPull(playerEntity, cmd);
                }
                actionInfo.IsThrow = true;
                actionInfo.ShowCountdownUI = false;
                //投掷时间
                actionInfo.LastFireTime = playerEntity.time.ClientTime;
                _throwingFactory.UpdateThrowing(actionInfo.ThrowingEntityKey, true, GetInitVel(playerEntity));
                //投掷动作
                playerEntity.stateInterface.State.FinishGrenadeThrow();
                //状态重置
                playerEntity.throwingUpdate.IsStartFly = false;
                //投掷型物品使用数量
                playerEntity.statisticsData.Statistics.UseThrowingCount++;
            }
        }

        private void CheckBrokeThrow(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var actionInfo = playerEntity.throwingAction.ActionInfo;
            if (actionInfo.IsReady && cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsThrowing))
            {
                //收回手雷
                playerEntity.GetController<PlayerWeaponController>().ForceUnmountCurrWeapon(_contexts);
                if (actionInfo.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _throwingFactory.DestroyThrowing(actionInfo.ThrowingEntityKey);
                }
                //拉栓未投掷，打断投掷动作
                playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
                actionInfo.ClearState();
            }
        }

        private float GetInitVel(PlayerEntity playerEntity)
        {
            var actionInfo = playerEntity.throwingAction.ActionInfo;
            return _throwingFactory.ThrowingInitSpeed(actionInfo.IsNearThrow);
        }
    }
}