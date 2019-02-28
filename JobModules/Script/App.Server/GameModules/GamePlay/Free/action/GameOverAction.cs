using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class GameOverAction : SendMessageAction
    {
        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.GameOver;
        }
    }
}
