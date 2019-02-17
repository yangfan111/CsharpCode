using Core.Utils;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultFireLogic" />
    /// </summary>
    public class DefaultFireLogic : IFireLogic
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultFireLogic));

        private List<IAfterFire> _afterFires = new List<IAfterFire>();

        private List<IFrame> _frames = new List<IFrame>();

        private List<IBeforeFireBullet> _beforeFires = new List<IBeforeFireBullet>();

        private List<IIdle> _idles = new List<IIdle>();

        private List<IFireCheck> _fireChecks = new List<IFireCheck>();

        private List<IFireTriggger> _fireTrigggers = new List<IFireTriggger>();

        private List<IBulletFire> _bulletFires = new List<IBulletFire>();

        public DefaultFireLogic()
        {
        }

        public void RegisterLogic<T>(T logic) where T : IForFireLogic
        {
            if (null == logic)
            {
                return;
            }
            var beforeLogic = logic as IBeforeFireBullet;
            if (null != beforeLogic)
            {
                _beforeFires.Add(beforeLogic);
            }
            var afterLogic = logic as IAfterFire;
            if (null != afterLogic)
            {
                _afterFires.Add(afterLogic);
            }
            var idleLogic = logic as IIdle;
            if (null != idleLogic)
            {
                _idles.Add(idleLogic);
            }
            var frameLogic = logic as IFrame;
            if (null != frameLogic)
            {
                _frames.Add(frameLogic);
            }
            var fireCheck = logic as IFireCheck;
            if (null != fireCheck)
            {
                _fireChecks.Add(fireCheck);
            }
            var fireTrigger = logic as IFireTriggger;
            if (null != fireTrigger)
            {
                _fireTrigggers.Add(fireTrigger);
            }
            var bulletFire = logic as IBulletFire;
            if (null != bulletFire)
            {
                _bulletFires.Add(bulletFire);
            }
        }

        public void ClearLogic()
        {
            _afterFires.Clear();
            _frames.Clear();
            _beforeFires.Clear();
            _idles.Clear();
            _fireChecks.Clear();
            _fireTrigggers.Clear();
            _bulletFires.Clear();
        }

        public void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            bool isFire = false;
            //判断是否有开火触发
            foreach (var fireTrigger in _fireTrigggers)
            {
                isFire |= fireTrigger.IsTrigger(controller, cmd);
            }
            if (isFire)
            {
                //判断是否有开火限制
                foreach (var fireCheck in _fireChecks)
                {
                    isFire &= fireCheck.IsCanFire(controller, cmd);
                }

            }
            if (isFire)
            {
                Fire(controller, cmd);
            }
            else
            {
                CallOnIdle(controller, cmd);
            }

            CallOnFrame(controller, cmd);
        }

        private void Fire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            CallBeforeFires(controller, cmd);
            CallBulletFires(controller, cmd);
            CallAfterFires(controller, cmd);
        }

        private void CallBulletFires(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var bulletfire in _bulletFires)
            {
                bulletfire.OnBulletFire(controller, cmd);
            }
        }

        private void CallAfterFires(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var afterfire in _afterFires)
            {
                afterfire.OnAfterFire(controller, cmd);
            }
        }

        private void CallBeforeFires(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            foreach (var beforeFire in _beforeFires)
            {
                beforeFire.BeforeFireBullet(controller, cmd);
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
            foreach (var beforeFire in _frames)
            {
                beforeFire.OnFrame(controller, cmd);
            }
        }
    }
}
