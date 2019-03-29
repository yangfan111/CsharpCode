using System;
using App.Client.Console.MessageHandler;
using App.Protobuf;
using com.wd.free.para;
using Core.GameModule.Step;
using Core.Utils;
using Sharpen;
using UnityEngine;


namespace App.Client.MessageHandler
{
    public class PingRespMessageHandler:AbstractClientMessageHandler<Protobuf.PingMessage>
    {
        private readonly Contexts _contexts;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PingRespMessageHandler));
       
        public PingRespMessageHandler(Contexts contexts)
        {
            _contexts = contexts;
           
        }

        public override void DoHandle(int messageType, PingMessage ping)
        {
            _contexts.session.clientSessionObjects.ServerFpsSatatus.AvgDelta = ping.AvgDelta;
            _contexts.session.clientSessionObjects.ServerFpsSatatus.MaxDelta = ping.MaxDelta;
            _contexts.session.clientSessionObjects.ServerFpsSatatus.Fps5 = ping.Fps5;
            _contexts.session.clientSessionObjects.ServerFpsSatatus.Fps30 = ping.Fps30;
            _contexts.session.clientSessionObjects.ServerFpsSatatus.Fps60 = ping.Fps60;
            _contexts.session.clientSessionObjects.ServerFpsSatatus.GcCount = ping.GcCount;
            long time = DateTime.UtcNow.ToMillisecondsSinceEpoch();
           // _logger.DebugFormat("{0} {1}",time%10000,ping.Time%10000);
            var p = (int) (time - ping.Time);
            if (ping.Type)
            {
                _contexts.session.clientSessionObjects.ServerFpsSatatus.TcpPing = p;
                _contexts.session.clientSessionObjects.ServerFpsSatatus.LastTcpPing = time;
            }
            else
            {
                _contexts.session.clientSessionObjects.ServerFpsSatatus.UdpPing = p;
                _contexts.session.clientSessionObjects.ServerFpsSatatus.LastUdpPing = time;
               
            }

           
//            if (ping.Fps5 > 15)
//            {
//                StepExecuteManager.Instance.UpdateCmdTargetFps(30);
//            }
//            else  if (ping.Fps5 > 10)
//            {
//                StepExecuteManager.Instance.UpdateCmdTargetFps(25);
//            }
//            else
//            {
//                StepExecuteManager.Instance.UpdateCmdTargetFps(20);
//            }
        }
    }
}