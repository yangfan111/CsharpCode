using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.buf
{
    [Serializable]
    public class DefinePlayerBufAction : AbstractPlayerAction
    {
        public int buf;
        public IGameAction startAction;
        public IGameAction action;
        public IGameAction endAction;

        public override void DoAction(IEventArgs args)
        {
            FreeData player = GetPlayer(args);

            if(player != null)
            {
                OnePlayerBuf buf = new OnePlayerBuf();
                buf.buf = this.buf;
                buf.startAction = (IGameAction)SerializeUtil.Clone(startAction);
                buf.action = (IGameAction)SerializeUtil.Clone(action);
                buf.endAction = (IGameAction)SerializeUtil.Clone(endAction);

                player.Bufs.RegisterPlayerBuf(buf);
            }
        }
    }
}
