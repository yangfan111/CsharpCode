using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Shared.FreeFramework.framework.buf
{
    [Serializable]
    public class AddEffectBufAction : AbstractPlayerAction, IRule
    {
        public string effect;
        public string level;
        public string time;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);
            int realTime = args.GetInt(time);
            if (realTime < 0)
            {
                fd.EffectBufs.RemoveEffect(args.GetString(effect), args.GetFloat(level));
            }
            else
            {
                fd.EffectBufs.AddEffect(args.GetString(effect), args.GetFloat(level), realTime, args);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.AddEffectBufAction;
        }
    }
}
