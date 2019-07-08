using System;
using App.Protobuf;
using App.Shared;
using Core.Network;
using Core.Utils;
using Utils.Singleton;


namespace App.Server.MessageHandler
{
    public class PingReqMessageHandler : AbstractServerMessageHandler<PlayerEntity, Protobuf.PingRequestMessage>
    {
        private Contexts _contexts;


        public override void DoHandle(INetworkChannel channel, PlayerEntity entity,
            EClient2ServerMessage eClient2ServerMessage,
            PingRequestMessage messageBody)
        {
            var serverStatus = _contexts.session.serverSessionObjects.FpsSatatus;
            var request = messageBody;
            var resp = Protobuf.PingMessage.Allocate();
            resp.Id = request.Id;
            resp.Type = request.Type;
            resp.Time = request.Time;
            resp.AvgDelta = (int) (serverStatus.AvgDelta);
            resp.MaxDelta = (int) (serverStatus.MaxDelta);
            resp.CmdLose = 100;
            resp.CmdTotal = 100;
            resp.Fps5 = serverStatus.Fps5;
            resp.Fps30 = serverStatus.Fps30;
            resp.Fps60 = serverStatus.Fps60;
            resp.GcCount = System.GC.CollectionCount(0);
            resp.GcStatus = SingletonManager.Get<gc_manager>().d_gc_disabled;
            resp.AllocatedMb = (int) SingletonManager.Get<gc_manager>().allocated_mb;
            resp.AllocationRateMbps = (int) SingletonManager.Get<gc_manager>().average_allocation_rate_mbps;
            if (request.Type)
            {
                channel.SendReliable((int) EServer2ClientMessage.Ping, resp);
            }
            else
            {
                channel.SendRealTime((int) EServer2ClientMessage.Ping, resp);
            }

            resp.ReleaseReference();
        }

        public PingReqMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            _contexts = contexts;
        }
    }
}