using com.wd.free.action;
using System;
using com.wd.free.@event;
using com.wd.free.util;
using App.Shared.GameModules.Bullet;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerHurtAction : AbstractGameAction
    {
        private string damage;
        private string type;
        private string part;
        private string source;
        private string target;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            PlayerEntity player = (PlayerEntity)fr.GetEntity(target);

            if (player != null)
            {
                if (player.gamePlay.IsDead())
                    return;

                PlayerEntity sourcePlayer = null;
                if (!string.IsNullOrEmpty(source))
                {
                    sourcePlayer = (PlayerEntity)fr.GetEntity(source);
                }

                if (string.IsNullOrEmpty(part))
                {
                    part = ((int)EBodyPart.Chest).ToString();
                }

                PlayerDamageInfo damageInfo = new PlayerDamageInfo(FreeUtil.ReplaceFloat(damage, args),
                    FreeUtil.ReplaceInt(type, args), FreeUtil.ReplaceInt(part, args), 0);

                BulletPlayerUtility.DoProcessPlayerHealthDamage(args.GameContext, (IGameRule)fr.Rule, sourcePlayer, player, damageInfo, null);
            }
        }
    }
}
