using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.ai;
using com.wd.free.@event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.wd.free.buf
{
    public class PlayerBuf
    {
        private OnePlayerBuf[] bufs;
        private int lastState;
        private PlayerEntity player;

        public PlayerBuf(PlayerEntity player)
        {
            this.bufs = new OnePlayerBuf[32];
            this.lastState = 0;

            this.player = player;
        }

        public void RegisterPlayerBuf(OnePlayerBuf buf)
        {
            bufs[buf.buf] = buf;
        }

        public void Frame(IEventArgs args)
        {
            int oldBufs = lastState;
            int newBufs = player.gamePlay.PlayerState;
            HandleCurrentBuf(args, player, oldBufs, newBufs);
            HandleRemoveBuf(args, player, oldBufs, newBufs);
            HandleAddBuf(args, player, oldBufs, newBufs);

            lastState = player.gamePlay.PlayerState;
        }

        private void HandleCurrentBuf(IEventArgs args, PlayerEntity player, int oldBufs, int newBufs)
        {
            for (int i = 1; i < bufs.Length; i++)
            {
                if ((newBufs & (1 << i)) > 0)
                {
                    if (bufs[i] != null && bufs[i].action != null)
                    {
                        args.Act(bufs[i].action, new TempUnit("current", (FreeData)player.freeData.FreeData));
                    }
                }
            }
        }

        private void HandleAddBuf(IEventArgs args, PlayerEntity player, int oldBufs, int newBufs)
        {
            for (int i = 1; i < bufs.Length; i++)
            {
                if ((newBufs & (1 << i)) > 0 && (oldBufs & (1 << i)) == 0)
                {
                    if (bufs[i] != null && bufs[i].startAction != null)
                    {
                        if (bufs[i].action != null)
                        {
                            bufs[i].action.Reset(args);
                        }
                        args.Act(bufs[i].startAction, new TempUnit("current", (FreeData)player.freeData.FreeData));
                    }
                }
            }
        }

        private void HandleRemoveBuf(IEventArgs args, PlayerEntity player, int oldBufs, int newBufs)
        {
            for (int i = 1; i < bufs.Length; i++)
            {
                if ((newBufs & (1 << i)) == 0 && (oldBufs & (1 << i)) > 0)
                {
                    if (bufs[i] != null && bufs[i].endAction != null)
                    {
                        args.Act(bufs[i].endAction, new TempUnit("current", (FreeData)player.freeData.FreeData));
                    }
                }
            }
        }
    }

    [Serializable]
    public class OnePlayerBuf
    {
        public int buf;
        public IGameAction startAction;
        public IGameAction action;
        public IGameAction endAction;
    }
}
