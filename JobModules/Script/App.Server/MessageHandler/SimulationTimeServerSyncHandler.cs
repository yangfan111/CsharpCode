using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Network;
using Core.Network;
using Core.Prediction.VehiclePrediction.TimeSync;

namespace App.Server.MessageHandler
{
    public class SimulationTimeServerSyncHandler : AbstractServerMessageHandler<PlayerEntity, SimulationTimeMessage>
    {
        private SimulationTimeSyncServer _server;
        public SimulationTimeServerSyncHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            _server = new SimulationTimeSyncServer(contexts.session.serverSessionObjects.SimulationTimer, Send);
            
        }

        private void Send(INetworkChannel channel, SimulationTimeMessage messageBody)
        {
            channel.SendReliable((int)EServer2ClientMessage.SimulationTimeSync, messageBody);
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, SimulationTimeMessage messageBody)
        {
            _server.OnSimulationTimeMessage(entity.network.NetworkChannel, messageBody);
            
        }
    }
}
