using App.Shared.Components.Player;
using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultBulletFireLogic" />
    /// </summary>
    public class DefaultBulletFireLogic : IBulletFire
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultBulletFireLogic));

        private BulletFireInfoProvider _bulletInfoProvider;

        private const int BulletCostInOneShot = 1;

        public DefaultBulletFireLogic()
        {
            _bulletInfoProvider = new BulletFireInfoProvider();
        }

        public void OnBulletFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {

            var bulletCfg = controller.HeldWeaponAgent.BulletCfg;
            AssertUtility.Assert(bulletCfg != null);
            var cmdSeq = cmd.CmdSeq;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;

            _bulletInfoProvider.Prepare(controller);
            // 射出子弹
            for (int i = 0; i < bulletCfg.HitCount; i++)
            {
                var bulletData = PlayerBulletData.Allocate();
                bulletData.Dir = _bulletInfoProvider.GetFireDir(cmdSeq + i, controller);
                weaponState.LastBulletDir = bulletData.Dir;
                bulletData.ViewPosition = _bulletInfoProvider.GetFireViewPosition(controller);
                bulletData.EmitPosition = _bulletInfoProvider.GetFireEmitPosition(controller);
                controller.AddAuxBullet(bulletData);
            }
            controller.AutoFire =0;
            controller.ExpendAfterAttack();
        }
    }
}
