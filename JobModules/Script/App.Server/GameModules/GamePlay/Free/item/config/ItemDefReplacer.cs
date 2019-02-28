using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.cpkf.yyjd.tools.util;

namespace App.Server.GameModules.GamePlay.Free.item.config
{
    public class ItemDefReplacer : IFreeReplacer
    {
        public bool CanHandle(string exp, IEventArgs args)
        {
            return exp.StartsWith("itemdef:");
        }

        public string Replace(string exp, IEventArgs args)
        {
            List<string> list = new List<string>();

            foreach (FreeItemInfo info in FreeItemConfig.ItemInfos)
            {
                list.Add(info.id + "***" + info.key + "***" + info.name + "***"
                    + (string.IsNullOrEmpty(info.type) ? " " : info.type) + "***"
                     + (string.IsNullOrEmpty(info.subType) ? " " : info.subType) + "***"
                      + (string.IsNullOrEmpty(info.fIcon) ? " " : info.fIcon) + "***"
                        + (string.IsNullOrEmpty(info.desc) ? " " : info.desc) + "***" + info.cat + "***"
                          + (string.IsNullOrEmpty(info.mIcon) ? " " : info.mIcon + "***" + info.stack));
            }
            return StringUtil.GetStringFromStrings(list, "@@@");
        }
    }
}
