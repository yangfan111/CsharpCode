using Core.Enums;
using WeaponConfigNs;
using Core.Utils;

namespace App.Shared.GameModules.Bullet
{
    /// <summary>
    /// 临时测试逻辑，用完删除
    /// </summary>
    public static class DamageInfoDebuger
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DamageInfoDebuger));
        private static bool _active = false;

        public static void OnEnvironmentHit(PlayerContext playerContext, IPlayerDamager damager, IDamageInfoCollector damageInfoCollector)
        {
            if(!_active)
            {
                return;
            }

            var player = playerContext.flagSelfEntity;
            BulletPlayerUtility.ProcessPlayerHealthDamage(
                damager,
                player,
                player, 
                new PlayerDamageInfo(10, (int)EUIDeadType.Weapon, (int)EBodyPart.Chest, player.weaponLogicInfo.WeaponId),
                damageInfoCollector);
        }

        private static int degree;
        public static void OnSendMessage(PlayerEntity player, Protobuf.PlayerDamageInfoMessage message)
        {
            if(!_active)
            {
                return;
            }
            var fakePos = player.position.Value;
            switch(degree % 4)
            {
                case 0:
                    fakePos += player.thirdPersonModel.Value.transform.forward;
                    break;
                case 1:
                    fakePos += player.thirdPersonModel.Value.transform.right;
                    break;
                case 2:
                    fakePos -= player.thirdPersonModel.Value.transform.forward;
                    break;
                case 3:
                    fakePos -= player.thirdPersonModel.Value.transform.right;
                    break;
            }
            message.PosX = fakePos.x;
            message.PosZ = fakePos.z;
            degree++;
        }
    }
}
