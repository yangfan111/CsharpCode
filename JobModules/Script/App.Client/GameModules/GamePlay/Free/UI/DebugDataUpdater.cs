using App.Shared;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class DebugDataUpdater : IUIUpdater
    {
        private bool disable;

        private long lastTime;

        private int count;

        private const int time = 10000;

        public bool IsDisabled
        {
            get { return disable; }
            set { disable = value; }
        }

        public void UIUpdate(int frameTime)
        {
            count++;
            long delta = Runtime.CurrentTimeMillis() - lastTime;
            if (delta > time)
            {
                SimpleProto data = FreePool.Allocate();

                data.Key = FreeMessageConstant.DebugData;

                IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());   //Dns.GetHostName()获取本机名Dns.GetHostAddresses()根据本机名获取ip地址组
                foreach (IPAddress ip in ips)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        data.Ss.Add(ip.ToString());
                        break;
                    }
                }

                PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;


                data.Ss.Add(SystemInfo.processorType);
                data.Ss.Add(SystemInfo.graphicsDeviceName);
                data.Ss.Add(player.position.Value.ToString());

                data.Ins.Add(SystemInfo.systemMemorySize);
                data.Ins.Add(SystemInfo.processorFrequency);
                data.Ks.Add(count * 1000 / (int)delta);

                if (!SharedConfig.IsOffline)
                {
                    SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
                }

                lastTime = Runtime.CurrentTimeMillis();
                count = 0;
            }
        }
    }
}
