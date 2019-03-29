using System;
using Core.Utils;
using UnityEngine;

namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public class ClientSimulationTimer : IClientSimulationTimer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<ClientSimulationTimer>.LoggerName);

        private readonly int MinAdvanceStep = 4;
        private readonly int TargetAdvenceStep = 10;
        private readonly int MaxAdvanceStep = 20; 
        private readonly int HalfAdvanceWindowSize = 5; 
        private readonly int ServerFrameCompensate = 10;
        private readonly int MaxLagTimeStep = 60;

        private readonly int MinStep = PhysicsTimeConfig.ClientStepMs;
        private readonly int MaxInterval = PhysicsTimeConfig.ClientStepMs * 2;

        private bool _enableCompensate;
        private int _intervalCompensate;
        private bool _lastSyncClientLag;

        private int _currentTime = Int32.MinValue;

        public int CurrentTime
        {
            get { return _currentTime < 0 ? 0 : _currentTime; }
            set { _currentTime = value; }

        }

        public int StepTime { get { return MinStep; } }

        public void StepCurrentTime()
        {
            _currentTime += MinStep;
        }

        public float SimulationStepTime { get { return MinStep * 0.001f; } }

        public SimulationTimeSyncInfo SyncInfo { get; set; }

        public ClientSimulationTimer(bool enableCompensate)
        {
            SyncInfo = new SimulationTimeSyncInfo();;
            _enableCompensate = enableCompensate;
        }

        public int ClampSimulationInterval(int interval)
        {
            interval = interval > MaxInterval ? MaxInterval : interval;
            interval += _intervalCompensate;

            if (interval < 0)
            {
                interval = 0;
            }

            return interval;
        }

        public void Sync()
        {
            
            if (!SyncInfo.IsWaitingSync)
            {
                return;;
            }

            SyncInfo.IsWaitingSync = false;

            if (CurrentTime <= 0)
            {
                var delay = SyncInfo.GetFirstDelayTime();

                var advanceStep = (int)Mathf.Ceil(delay / StepTime);
                var advanceTime = (Mathf.Clamp(advanceStep, MinAdvanceStep, MaxAdvanceStep) + ServerFrameCompensate) * StepTime;
                var expectedTime = SyncInfo.FirstRecvServerTime + advanceTime;
                CurrentTime = expectedTime;
                _intervalCompensate = 0;
            }
            else if(_enableCompensate)
            {
                var deltaTime = SyncInfo.LastSyncTimeDelta;

                var minAdanceTime = MinAdvanceStep * StepTime;
                var maxAdanceTime = MaxAdvanceStep * StepTime;
                var targetAdanceTime = TargetAdvenceStep * StepTime;
                var windowTime = HalfAdvanceWindowSize * StepTime;
               
                var largeStep = StepTime * 2;
                var smallStep = StepTime;
                if (deltaTime < 0)
                {
                    _intervalCompensate = -deltaTime + TargetAdvenceStep + ServerFrameCompensate * StepTime;
                    _logger.InfoFormat("ClientSync, the server simulation Time Outpace client {0}", deltaTime);

                    var maxLagTime = MaxLagTimeStep * StepTime;
                    if (_intervalCompensate < maxLagTime)
                    {
                        _intervalCompensate = maxLagTime;
                    }
                    _lastSyncClientLag = true;
                }
                else
                {
                    if (_lastSyncClientLag)
                    {
                        _intervalCompensate = 0;
                        _lastSyncClientLag = false;
                    }

                    if (deltaTime < minAdanceTime)
                    {
                        //speed up with a large step
                        if (_intervalCompensate < 0)
                        {
                            _intervalCompensate = 0;
                        }

                        _intervalCompensate += largeStep;

                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugFormat(
                                "SpeedUp Large SimulationTime Compensate {0}, deltaTime {1}, Min {2}, TMin {3}, TMax {4}, Max {5}",
                                _intervalCompensate, deltaTime, minAdanceTime, targetAdanceTime - windowTime,
                                targetAdanceTime + windowTime, maxAdanceTime);

                        }
                    }
                    else if (deltaTime < targetAdanceTime - windowTime)
                    {
                        //speed up with a small step
                        if (_intervalCompensate < 0)
                        {
                            _intervalCompensate = 0;
                        }
                        _intervalCompensate += smallStep;
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugFormat(
                                "SpeedUp Small SimulationTime Compensate {0}, deltaTime {1}, Min {2}, TMin {3}, TMax {4}, Max {5}",
                                _intervalCompensate, deltaTime, minAdanceTime, targetAdanceTime - windowTime,
                                targetAdanceTime + windowTime, maxAdanceTime);
                        }
                    }
                    else if (deltaTime <= targetAdanceTime + windowTime)
                    {
                        _intervalCompensate = 0;
                    }
                    else if (deltaTime <= maxAdanceTime)
                    {
                        //speed Down with a small step
                        if (_intervalCompensate > 0)
                        {
                            _intervalCompensate = 0;
                        }
                        _intervalCompensate -= smallStep;
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("SpeedDown Small SimulationTime Compensate {0}, deltaTime {1}, Min {2}, TMin {3}, TMax {4}, Max {5}",
                            _intervalCompensate, deltaTime, minAdanceTime, targetAdanceTime - windowTime, targetAdanceTime + windowTime, maxAdanceTime);
                    }
                    else
                    {
                        //speed down
                        if (_intervalCompensate > 0)
                        {
                            _intervalCompensate = 0;
                        }

                        _intervalCompensate -= largeStep;
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("SpeedDown Large SimulationTime Compensate {0}, deltaTime {1}, Min {2}, TMin {3}, TMax {4}, Max {5}",
                            _intervalCompensate, deltaTime, minAdanceTime, targetAdanceTime - windowTime, targetAdanceTime + windowTime, maxAdanceTime);
                    }
                }
            }




                
        }
    }
}
