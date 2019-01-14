using System;
using Core.Utils;

namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public class SimulationTimeSyncClient
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<SimulationTimeSyncClient>.LoggerName);
        private IClientSimulationTimer _simulationTimer;
        private bool _firstSend = true;
        private bool _firstRecv = true;
        private Action<SimulationTimeMessage> _sendMethod;
        private bool _serverAuthorative;
        public SimulationTimeSyncClient(IClientSimulationTimer simulationTimer, Action<SimulationTimeMessage> sendMethod, bool serverAuthorative)
        {
            _simulationTimer = simulationTimer;
            _sendMethod = sendMethod;
            _serverAuthorative = serverAuthorative;
        }

        public void Update()
        {
            if (!_serverAuthorative)
            {
                if (_simulationTimer.CurrentTime == 0)
                {
                    _simulationTimer.CurrentTime = _simulationTimer.StepTime;
                }

                return;
            }

            if (_firstSend)
            {
                _simulationTimer.SyncInfo.SetFirstSendTime(DateTime.Now);
                _firstSend = false;
            }


            var msg = new SimulationTimeMessage()
            {
                ClientSimulationTime = _simulationTimer.CurrentTime,
                ServerSimulationTime = 0
            };
            _sendMethod.Invoke(msg);
            

            _simulationTimer.Sync();
        }

        public void OnSimulationTimeMessage(SimulationTimeMessage messageBody)
        {

            var syncInfo = _simulationTimer.SyncInfo;
            if (_firstRecv)
            {
                syncInfo.SetFirstRecvTime(DateTime.Now);
                syncInfo.FirstRecvServerTime = messageBody.ServerSimulationTime;
                _firstRecv = false;
            }
            else if(messageBody.ClientSimulationTime <= 0)
            {
                return;
            }

            var delta = messageBody.ClientSimulationTime - messageBody.ServerSimulationTime;
            syncInfo.LastSyncTimeDelta = delta;
            syncInfo.IsWaitingSync = true;

            if (delta < 0)
            {
                _logger.DebugFormat("ClientSync The Server Outpace Client delta {0}", delta);
            }
        }
    }
}