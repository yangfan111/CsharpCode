using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using gameplay.gamerule.free.ui.component;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.framework.ui
{
    [Serializable]
    public class FreeUiDuplicateAction : SendMessageAction
    {
        public string type;
        public string key;

        public string x;
        public string y;
        public string relative;

        public string show;

        private List<IFreeUIValue> values;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.DuplicateUI;

            builder.Ss.Add(FreeUtil.ReplaceVar(key, args) + FreeMessageConstant.SpilterField + FreeUtil.ReplaceVar(type, args));

            builder.Fs.Add(FreeUtil.ReplaceFloat(x, args));
            builder.Fs.Add(FreeUtil.ReplaceFloat(y, args));

            builder.Bs.Add(FreeUtil.ReplaceBool(show, args));

            builder.Ks.Add(FreeUtil.ReplaceInt(relative, args));

            if (values != null)
            {
                foreach (IFreeUIValue com in values)
                {
                    builder.Ks.Add(com.GetSeq(args));
                    builder.Ins.Add(com.GetAutoStatus() * 100 + com.GetAutoIndex());
                }
                foreach (IFreeUIValue com_1 in values)
                {
                    object obj = com_1.GetValue(args);
                    if (obj == null)
                    {
                        builder.Ss.Add("null");
                    }
                    else
                    {
                        builder.Ss.Add(obj.ToString());
                    }
                }
            }
        }
    }
}
