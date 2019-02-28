using com.wd.free.action;
using com.wd.free.@event;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    class PlayerBagLockAction : AbstractPlayerAction
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBagLockAction));
        bool islock;
        int duration;
        public override void DoAction(IEventArgs args)
        {
            var player = GetPlayerEntity(args);
            if(null == player)
            {
                Logger.Error("player is null");
                return;
            }
            if(!player.hasWeaponState)
            {
                Logger.Error("player has no weapon state");
                return;
            }
            player.weaponState.BagLocked = islock;
            if (duration > 0)
            {
                if (!player.hasTime)
                {
                    Logger.Error("player has no time component");
                    return;
                }

                player.weaponState.BagOpenLimitTime = player.time.ClientTime + duration;
            }
        }
    }
}
