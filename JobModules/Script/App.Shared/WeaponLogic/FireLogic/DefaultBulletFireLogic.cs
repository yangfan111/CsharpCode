using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon.Bullet;
using Core;
using Core.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace App.Shared.WeaponLogic.FireLogic
{
    public class DefaultBulletFireLogic : IBulletFire
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultBulletFireLogic));
        private BulletFireInfoProvider _bulletInfoProvider;
        private const int BulletCostInOneShot = 1;
        private Contexts _contexts;

        public DefaultBulletFireLogic(Contexts contexts)
        {
            _contexts = contexts;
            _bulletInfoProvider = new BulletFireInfoProvider(contexts);
        } 

        public void OnBulletFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var bulletConfig = GetConfig(playerEntity);
            if(null == bulletConfig)
            {
                LogError("bulletConfig is null");
                return;
            }
            var cmdSeq = cmd.CmdSeq;
            var weaponState = weaponEntity.weaponData;

            _bulletInfoProvider.Prepare(playerEntity);
            // 射出子弹
            for (int i = 0; i < bulletConfig.HitCount; i++)
            {
                var bulletData = PlayerBulletData.Allocate();
                bulletData.Dir = _bulletInfoProvider.GetFireDir(cmdSeq + i, playerEntity, weaponEntity);
                weaponState.LastBulletDir = bulletData.Dir;
                bulletData.ViewPosition = _bulletInfoProvider.GetFireViewPosition(playerEntity);
                bulletData.EmitPosition = _bulletInfoProvider.GetFireEmitPosition(playerEntity);
                playerEntity.playerBulletData.DataList.Add(bulletData);
            }
            var playerWeaponController = playerEntity.GetController<PlayerWeaponController>();
            if(null != playerWeaponController)
            {
                playerWeaponController.OnExpend(_contexts, (EWeaponSlotType)playerEntity.bagState.CurSlot);
            }
            if (playerEntity.hasWeaponAutoState)
            {
                playerEntity.weaponAutoState.AutoFire = 0;
            }
        }

        private BulletConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.BulletCfg;
            }
            return null;
        }

        private void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}
