using App.Shared;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    class PlayerBagLockAction : AbstractPlayerAction, IRule
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBagLockAction));
        bool islock;
        string duration;
        public override void DoAction(IEventArgs args)
        {
            var player = GetPlayerEntity(args);
            if(null == player)
            {
                Logger.Error("player is null");
                return;
            }
            player.WeaponController().BagLockState = islock;
            int v = args.GetInt(duration);
            if (player.hasTime && v > 0)
                player.WeaponController().BagOpenLimitTIme = player.time.ClientTime + v;
           
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerBagLockAction;
        }
    }
}
