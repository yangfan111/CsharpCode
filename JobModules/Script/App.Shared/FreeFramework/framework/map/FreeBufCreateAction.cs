using System;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;

namespace com.wd.free.map
{
    [System.Serializable]
    public class FreeBufCreateAction : AbstractPlayerAction
    {
        private const long serialVersionUID = 6850781259702774772L;

        private IList<FreeBuf> bufs;

        public override void DoAction(IEventArgs args)
        {
            FreeData targetPlayer = ((FreeData)args.GetUnit(this.player));
            if (bufs != null)
            {
                foreach (FreeBuf buf in bufs)
                {
                    FreeBuf copy = (FreeBuf)SerializeUtil.Clone(buf);
                    if (targetPlayer != null)
                    {
                        copy.SetCreator(targetPlayer.Player);
                    }
                    copy.OnCreate(args);
                    args.TempUse("buf", copy);
                    try
                    {
                        args.FreeContext.Bufs.AddFreeBuf(copy, args);
                    }
                    catch (Exception e)
                    {
                        Sharpen.Runtime.PrintStackTrace(e);
                    }
                    args.Resume("buf");
                }
            }
        }

        public void AddFreeBuf(FreeBuf buf)
        {
            if (bufs == null)
            {
                bufs = new List<FreeBuf>();
            }

            bufs.Add(buf);
        }
    }
}
