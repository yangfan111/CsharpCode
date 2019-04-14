using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.framework.buf
{
    [Serializable]
    public class DefineEffectBufAction : AbstractGameAction
    {
        public string key;
        public int effectType;

        public override void DoAction(IEventArgs args)
        {
            EffectBuf buf = new EffectBuf();
            buf.key = args.GetString(key);
            buf.type = (EffectType)effectType;

            PlayerEffectBuf.RegisterEffectBuf(buf);
        }
    }
}
