using App.Shared.Components.Player;
using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="BulletFireEventListener" />
    /// </summary>
    public class BulletFireEventListener : IBulletFireListener
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BulletFireEventListener));

        private BulletFireInfoProvider _bulletInfoProvider;

        private const int BulletCostInOneShot = 1;

        public BulletFireEventListener()
        {
            _bulletInfoProvider = new BulletFireInfoProvider();
        }

        public void OnBulletFire(PlayerWeaponController controller, WeaponSideCmd cmd)
        {

            var bulletCfg = controller.HeldWeaponAgent.BulletCfg;
            AssertUtility.Assert(bulletCfg != null);
            var cmdSeq = cmd.UserCmd.Seq;
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;

            _bulletInfoProvider.Prepare(controller);
            // 射出子弹
            for (int i = 0; i < bulletCfg.HitCount; i++)
            {
                var bulletData = PlayerBulletData.Allocate();
                bulletData.Dir = _bulletInfoProvider.GetFireDir(cmdSeq + i, controller);
                runTimeComponent.LastBulletDir = bulletData.Dir;
                bulletData.ViewPosition = _bulletInfoProvider.GetFireViewPosition(controller);
                bulletData.EmitPosition = _bulletInfoProvider.GetFireEmitPosition(controller);
                controller.AddAuxBullet(bulletData);
            }
            controller.AfterAttack();
        }
    }
}
