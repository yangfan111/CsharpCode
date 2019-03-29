using System;
using Core.Utils;

namespace Core.Prediction.VehiclePrediction.TimeSync
{
    //for  AutoSimulation == false Timer
    public class ServerSimulationTimer : ISimulationTimer
    {
        private readonly int MinStep = PhysicsTimeConfig.ServerStepMs;
        private readonly int MaxInterval = PhysicsTimeConfig.ServerStepMs * 3;

        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<ServerSimulationTimer>.LoggerName);

        private int _currentTime = Int32.MinValue;

        public int CurrentTime {
            get { return _currentTime < 0 ? 0 : _currentTime; }
            set { _currentTime = value; }
            
        }

        public int StepTime { get { return MinStep; } }

        public void StepCurrentTime()
        {
            _currentTime += MinStep;
        }

        public float SimulationStepTime { get { return MinStep * 0.001f; } }

        public virtual int ClampSimulationInterval(int interval)
        {
            return interval > MaxInterval ? MaxInterval : interval;
        }
    }
}
