using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.ui
{
    [Serializable]
    public class ShowPartAction : SendMessageAction
    {
        public bool show;

        public string weaponKey;

        public string parts;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.AddChild;

            builder.Ks.Add(5);

            builder.Bs.Add(show);

            builder.Ins.Add(FreeUtil.ReplaceInt(weaponKey, args));

            if(parts == null)
            {
                parts = "";
            }
            builder.Ss.Add(parts);
        }
    }
}
