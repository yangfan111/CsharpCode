using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ItemInfoHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ItemInfo;
        }

        public void Handle(SimpleProto data)
        {
            string key = data.Ss[0];

            TipData tip = new TipData(data.Ins[0], data.Ins[1], data.Ins[2]);

            TipUtil.AddTip(key, tip);
        }
    }
}
