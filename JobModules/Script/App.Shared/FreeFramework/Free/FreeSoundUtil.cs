using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.Free
{
    public class FreeSoundUtil
    {
        public  static void Stop(string key, IEventArgs args, FreeData fd)
        {
            SimpleProto builder = FreePool.Allocate();
            builder.Key = FreeMessageConstant.PlaySound;

            builder.Bs.Add(true);
            builder.Bs.Add(false);
            builder.Bs.Add(false);

            builder.Ins.Add(0);

            builder.Ss.Add(key);

            fd.Player.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, builder);
        }

        public static void PlayOnce(string key, int id, IEventArgs args, FreeData fd)
        {
            SimpleProto builder = FreePool.Allocate();
            builder.Key = FreeMessageConstant.PlaySound;

            builder.Bs.Add(false);
            builder.Bs.Add(false);
            builder.Bs.Add(false);

            builder.Ins.Add(id);

            builder.Ss.Add(key);

            fd.Player.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, builder);
        }
    }
}
