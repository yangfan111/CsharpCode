using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.framework.action
{
    [Serializable]
    public class DebugFreeAction : AbstractGameAction
    {
        public bool debug;
        public string fields;

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
        }
    }
}
