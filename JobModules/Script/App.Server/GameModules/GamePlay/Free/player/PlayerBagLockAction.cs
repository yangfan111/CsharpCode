using App.Shared;
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
            player.WeaponController().BagLockState = islock;
            if(player.hasTime && duration>0)
                player.WeaponController().BagOpenLimitTIme = player.time.ClientTime + duration;
           
        }
    }
}
