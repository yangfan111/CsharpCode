using App.Server.GameModules.GamePlay;
using Core.EntityComponent;
using Core.Utils;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon.Behavior
{

    /// <summary>
    /// Defines the <see cref="CommonWeaponFireController" />
    /// </summary>
    public class CommonWeaponFireController :AbstractFireController
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonWeaponFireController));

        private List<IAfterFireProcess> _afterFireProcessors = new List<IAfterFireProcess>();

        private List<IFrameProcess> _frames = new List<IFrameProcess>();

        private List<IBeforeFireProcess> _beforeFireProcessors = new List<IBeforeFireProcess>();

        private List<IIdleProcess> _idles = new List<IIdleProcess>();

        private List<IFireChecker> _fireCheckers = new List<IFireChecker>();

        private List<IFireTriggger> _fireTrigggers = new List<IFireTriggger>();

        private List<IBulletFireListener> _bulletFires = new List<IBulletFireListener>();

        public CommonWeaponFireController()
        {
        }

        public void RegisterProcessor<T>(T logic) where T : IFireProcess
        {
            if (null == logic)
            {
                return;
            }
            var beforeLogic = logic as IBeforeFireProcess;
            if (null != beforeLogic)
            {
                _beforeFireProcessors.Add(beforeLogic);
            }
            var afterLogic = logic as IAfterFireProcess;
            if (null != afterLogic)
            {
                _afterFireProcessors.Add(afterLogic);
            }
            var idleLogic = logic as IIdleProcess;
            if (null != idleLogic)
            {
                _idles.Add(idleLogic);
            }
            var frameLogic = logic as IFrameProcess;
            if (null != frameLogic)
            {
                _frames.Add(frameLogic);
            }
            var fireCheck = logic as IFireChecker;
            if (null != fireCheck)
            {
                _fireCheckers.Add(fireCheck);
            }
            var fireTrigger = logic as IFireTriggger;
            if (null != fireTrigger)
            {
                _fireTrigggers.Add(fireTrigger);
            }
            var bulletFire = logic as IBulletFireListener;
            if (null != bulletFire)
            {
                _bulletFires.Add(bulletFire);
            }
        }

        public void ClearLogic()
        {
            _afterFireProcessors.Clear();
            _frames.Clear();
            _beforeFireProcessors.Clear();
            _idles.Clear();
            _fireCheckers.Clear();
            _fireTrigggers.Clear();
            _bulletFires.Clear();
        }

        protected override void UpdateFire(PlayerWeaponController controller, IWeaponCmd cmd,Contexts contexts)
        {
            bool isFire = false;
            
            foreach (var fireTrigger in _fireTrigggers)
            {
                isFire |= fireTrigger.IsTrigger(controller, cmd);
            }
            if (isFire)
            {
                foreach (var fireCheck in _fireCheckers)
                {
                    isFire &= fireCheck.IsCanFire(controller, cmd);
                }

            }
            if (isFire && controller.RelatedThrowAction.ThrowingEntityKey == EntityKey.Default
                && (controller.RelatedThrowAction.LastFireWeaponKey == controller.HeldWeaponAgent.WeaponKey.EntityId || controller.RelatedThrowAction.LastFireWeaponKey == 0))
            {
                Fire(controller, cmd,contexts);
                controller.RelatedThrowAction.LastFireWeaponKey = controller.HeldWeaponAgent.WeaponKey.EntityId;
            }
            else
            {
                CallOnIdle(controller, cmd);
            }

            CallOnFrame(controller, cmd);
        }


        private void Fire(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
        {
            CallBeforeFires(controller, cmd);
            CallBulletFires(controller, cmd,contexts);
            CallAfterFires(controller, cmd);
        }

        private void CallBulletFires(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
        {
            foreach (var bulletfire in _bulletFires)
            {
                bulletfire.OnBulletFire(controller, cmd);
            }

            if (SharedConfig.IsServer)
            {
                FreeRuleEventArgs args = contexts.session.commonSession.FreeArgs as FreeRuleEventArgs;
                (args.Rule as IGameRule).HandleWeaponFire(contexts, contexts.player.GetEntityWithEntityKey(controller.Owner), controller.GetWeaponAgent().ResConfig);
            }
        }

        private void CallAfterFires(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var afterfire in _afterFireProcessors)
            {
                afterfire.OnAfterFire(controller, cmd);
            }
        }

        private void CallBeforeFires(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var beforeFire in _beforeFireProcessors)
            {
                beforeFire.OnBeforeFire(controller, cmd);
            }
        }

        private void CallOnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var fireIdle in _idles)
            {
                fireIdle.OnIdle(controller, cmd);
            }
        }

        private void CallOnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var frame in _frames)
            {
                frame.OnFrame(controller, cmd);
            }
        }
    }
}
