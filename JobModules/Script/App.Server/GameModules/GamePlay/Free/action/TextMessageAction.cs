using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using Sharpen;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class TextMessageAction : SendMessageAction
    {
        private string message;
        private int interval;

        [NonSerialized]
        private long lastTime;

        [NonSerialized]
        private FreeUITextValue textValue;
        private FreeUIUpdateAction update;
        private FreeUIShowAction show;

        public override void DoAction(IEventArgs args)
        {
            if (update == null)
            {
                update = new FreeUIUpdateAction();
                update.SetKey("testUI");
                update.SetPlayer(FreeUtil.ReplaceVar(player, args));
                update.SetScope(FreeUtil.ReplaceInt(this.scope, args));

                textValue = new FreeUITextValue();
                textValue.SetSeq("1");

                update.AddValue(textValue);

                show = new FreeUIShowAction();
                show.SetKey("testUI");
                show.SetPlayer(FreeUtil.ReplaceVar(player, args));
                show.SetScope(FreeUtil.ReplaceInt(this.scope, args));
                show.SetTime("2000");

                if (interval == 0)
                {
                    interval = 1000;
                }
            }

            if (Runtime.CurrentTimeMillis() - lastTime >= interval)
            {
                textValue.SetText(FreeUtil.ReplaceVar(message, args));

                update.DoAction(args);
                show.DoAction(args);

                lastTime = Runtime.CurrentTimeMillis();
            }
        }

        protected override void BuildMessage(IEventArgs args)
        {

        }
    }
}
