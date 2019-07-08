using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Shared.FreeFramework.framework.action
{
    [Serializable]
    public class DebugFreeAction : AbstractGameAction, IRule
    {
        public bool debug;
        public string fields;
        public bool log;

        public override void DoAction(IEventArgs args)
        {
            if (debug)
            {
                FreeLog.Enable();
            }
            else
            {
                FreeLog.Disable();
            }
            
            if (!string.IsNullOrEmpty(fields))
            {
                FreeLog.SetParas(fields);
            }

            args.FreeContext.DebugMode = log;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.DebugFreeAction;
        }
    }
}
