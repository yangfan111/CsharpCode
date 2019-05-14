using com.wd.free.action;
using com.wd.free.@event;
using System;

namespace App.Shared.FreeFramework.framework.action
{
    [Serializable]
    public class DebugFreeAction : AbstractGameAction
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
    }
}
