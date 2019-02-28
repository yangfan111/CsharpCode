using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Free.framework;
using com.cpkf.yyjd.tools.util;
using UnityEngine;
using Core.Utils;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeMessageHandler
    {
        private static List<IFreeMessageHandler> handlers;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FreeMessageHandler));

        static FreeMessageHandler()
        {
            handlers = new List<IFreeMessageHandler>();
            handlers.Add(new FreeLoginSucessHandler());
            handlers.Add(new FreeClickImageHandler());
            handlers.Add(new FreeMoveImageHandler());
            handlers.Add(new FreePickupHandler());
            handlers.Add(new FreeDragImageHandler());
            handlers.Add(new FreeAddMarkHandler());
            handlers.Add(new FreeDebugDataHandler());
            handlers.Add(new SplitItemHandler());
            handlers.Add(new FreeObservePlayerHandler());
        }

        public static void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {

            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i].CanHandle(room, player, message))
                {
                    try
                    {
                        handlers[i].Handle(room, player, message);
                    }
                    catch (Exception e)
                    {
                        string err = "handle client message failed\nat    " + handlers[i].ToString() + "\nat    " + message.ToString() + "\nat    " + ExceptionUtil.GetExceptionContent(e);
                        Debug.LogError(err);
                        _logger.Error(err);
                    }
                }
            }
        }
    }
}
