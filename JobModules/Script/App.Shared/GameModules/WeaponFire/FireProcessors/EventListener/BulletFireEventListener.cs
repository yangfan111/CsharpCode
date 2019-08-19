using App.Shared.Components.Player;
using Core.Components;
using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="BulletFireEventListener" />
    /// </summary>
    public class BulletFireEventListener : IBulletFireListener
    {
        private const int BulletCostInOneShot = 1;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BulletFireEventListener));
        private IAmmunitionCalculator normalFireAmmu = new NormalFireAmmunitionCalculator();
        private IAmmunitionCalculator sightFireAmmu = new SightFireAmmunitionCalculator();


        public void OnBulletFire(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            //            DebugUtil.MyLog("[seq:{1}]MuzzleP3Position:{0}",controller.RelatedFirePos.MuzzleP3Position,cmd.UserCmd.Seq);
            var bulletCfg = controller.HeldWeaponAgent.BulletCfg;
            AssertUtility.Assert(bulletCfg != null);
            var cmdSeq   = cmd.UserCmd.Seq;
            var fireAmmu = controller.RelatedCameraSNew.IsAiming() ? sightFireAmmu : normalFireAmmu;
            // 射出子弹
            for (int i = 0; i < bulletCfg.HitCount; i++)
            {
                var bulletData = PlayerBulletData.Allocate();
                bulletData.Dir =
                                PrecisionsVector3.MakePrecisionsV(
                                    fireAmmu.GetFireDir(cmdSeq + i, controller, cmd.UserCmd.Seq), 3);
                var viewPos = PrecisionsVector3.MakePrecisionsV(fireAmmu.GetFireViewPosition(controller), 3);
                bulletData.ViewPosition = PrecisionsVector3.MakeRoundPrecisionsV(viewPos, 3);
                //枪口位置取原始精度
                bulletData.EmitPosition = fireAmmu.GetFireEmitPosition(controller);
                controller.AddAuxBullet(bulletData);
                // DebugUtil.AppendShootText(cmd.UserCmd.Seq, "[Bullet Fire]{0}", bulletData.ToStringExt());
            }

            controller.AfterAttack();
        }
    }
}