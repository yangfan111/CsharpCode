using com.wd.free.action;
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
    public class ShowAppUiAction : SendMessageAction
    {
        private string show;
        private string ui;

        protected override void BuildMessage(IEventArgs args)
        {
            bool realShow = FreeUtil.ReplaceBool(show, args);
            int realUi = FreeUtil.ReplaceInt(ui, args); 

            if(realUi == -1)
            {
                builder.Key = FreeMessageConstant.LockMouse;
                builder.Bs.Add(!realShow);
            }
            else
            {
                builder.Key = FreeMessageConstant.ShowCodeUI;
                builder.Ks.Add(realUi);
                builder.Bs.Add(realShow);
            }
            
        }
    }
}
