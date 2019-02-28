using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.Console.MessageHandler;
using App.Client.MessageHandler;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Network;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction.TimeSync;


namespace App.Client.GameModules.UserPhysics
{
    public class SimulationTimeSyncClientSystem : AbstractClientMessageHandler<SimulationTimeMessage>, IPhysicsUpdateSystem
    {

        private ClientSessionObjectsComponent _sessionObjects;
        private SimulationTimeSyncClient _simulationTimeSyncClient;

        
        public SimulationTimeSyncClientSystem(Contexts contexts)
        {
            var sessionObjects = contexts.session.clientSessionObjects;
            _sessionObjects = sessionObjects;
            _simulationTimeSyncClient = new SimulationTimeSyncClient(
                sessionObjects.SimulationTimer, Send,
                SharedConfig.ServerAuthorative
                );

            sessionObjects.MessageDispatcher.RegisterLater((int)EServer2ClientMessage.SimulationTimeSync, this);
        }

        public void Send(SimulationTimeMessage msg)
        {
            var channel = _sessionObjects.NetworkChannel;
            if (channel == null)
            {
                return;
            }

            channel.SendRealTime((int)EClient2ServerMessage.SimulationTimeSync, msg);
        }

        public void Update()
        {
            if (PhysicsUtility.IsAutoSimulation)
            {
                return;
            }

            _simulationTimeSyncClient.Update();
            
        }

        public override void DoHandle(int messageType, SimulationTimeMessage messageBody)
        {
            _simulationTimeSyncClient.OnSimulationTimeMessage(messageBody);
        }
    }
}
