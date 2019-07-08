using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerMaskAction : AbstractPlayerAction, IRule
    {
        public string SelfMask;
        public string TargetMask;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity p = GetPlayer(args).Player;
            p.playerMask.SelfMask = (byte)args.GetInt(SelfMask);
            p.playerMask.TargetMask = (byte)args.GetInt(TargetMask);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerMaskAction;
        }
    }
}