using System;
using Core.Network;

namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public class SimulationTimeSyncServer
    {
        private ISimulationTimer _simulationTimer;
        private Action<INetworkChannel, SimulationTimeMessage> _sendMethod;

        public SimulationTimeSyncServer(ISimulationTimer simulationTimer, Action<INetworkChannel, SimulationTimeMessage> sendMethod)
        {
            _simulationTimer = simulationTimer;
            _sendMethod = sendMethod;
        }

        public void OnSimulationTimeMessage(INetworkChannel channel, SimulationTimeMessage msg)
        {
            msg.ServerSimulationTime = _simulationTimer.CurrentTime;
            _sendMethod.Invoke(channel, msg);

        }
    }
}