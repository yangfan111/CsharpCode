using System;
using System.Collections.Generic;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Utils;
using Entitas;
using UnityEngine.Profiling;
using Utils.Singleton;
using Utils.Utils;

namespace Core.Prediction.VehiclePrediction
{
    public class VehicleCmdExecuteManagerSystem : AbstractFrameworkSystem<IVehicleCmdExecuteSystem>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<VehicleCmdExecuteManagerSystem>.LoggerName);
        private IVehicleCmdExecuteSystemHandler _handler;
        private IList<IVehicleCmdExecuteSystem> _systems;
        private IVehicleCmd _currentCmd;
        private Entity _currentEntity;
        private IVehicleExecutionSelector _vehicleSelector;

        private bool _isServer;
        private bool _serverAuthorative = true;


        private bool _firstStep;
        private bool _lastStep;

        private ISimulationTimer _simulationTimer;
        private DateTime _lastRealTime = DateTime.MinValue;
        private int _interval;

        enum Stage
        {
            UpdateSimulationTime,
            SyncFromComponent,
            ExecuteCmds,
            FixedUpdate,
            Update,
            SyncToComponent,
        };

        private Stage _stage;


        private ICustomProfiler _profiler;

        public VehicleCmdExecuteManagerSystem(IVehicleExecutionSelector vehicleSelector, IGameModule gameModule, IVehicleCmdExecuteSystemHandler handler, ISimulationTimer simulationTimer, bool isServer, bool serverAuthorative)
        {
            _systems = gameModule.VehicleCmdExecuteSystems;
            _handler = handler;
            _simulationTimer = simulationTimer;
            _vehicleSelector = vehicleSelector;
            _isServer = isServer;
            _serverAuthorative = serverAuthorative;
            _profiler = CustomProfilerFactory.CreateProfiler(this);
            Init();
        }

        public override IList<IVehicleCmdExecuteSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IVehicleCmdExecuteSystem system)
        {
            if(_stage == Stage.UpdateSimulationTime)
                system.UpdateSimulationTime(_handler.LastSimulationTime);
            else if (_stage == Stage.SyncFromComponent)
                system.SyncFromComponent(_currentEntity);  
            else if (_stage == Stage.SyncToComponent)
                system.SyncToComponent(_currentEntity); 
            else if (_stage == Stage.FixedUpdate)
                system.FixedUpdate(_currentEntity);
            else if(_stage == Stage.Update)
                system.Update(_currentEntity);
            else 
                system.ExecuteVehicleCmd(_currentEntity, _currentCmd);
        }

        public override void Execute()
        {
            if(!isStepExecute()) return;

            try
            {
                StartProfiler(CustomProfilerStep.VechiclePrediction);
                _lastStep = true;

                if (!_handler.IsReady())
                {
                    return;
                }

                if (_lastRealTime == DateTime.MinValue)
                {
                    _lastRealTime = DateTime.Now;
                    return;
                }

                var currentRealTime = DateTime.Now;
                _interval += (int) (currentRealTime - _lastRealTime).TotalMilliseconds;
                _lastRealTime = currentRealTime;
                _interval = _simulationTimer.ClampSimulationInterval(_interval);

                while (_interval >= _simulationTimer.StepTime)
                {
                    _simulationTimer.StepCurrentTime();
                    _interval -= _simulationTimer.StepTime;
                }

                
                if (_handler.LastSimulationTime < _simulationTimer.CurrentTime)
                {
                    _vehicleSelector.Update();
                    SingletonManager.Get<DurationHelp>().ProfileAddInfo(CustomProfilerStep.VehicleFixedUpdate,
                        _vehicleSelector.ActiveCount.ToString());
                }

                _firstStep = true;

                int executeCount = 0;
                while (_handler.LastSimulationTime < _simulationTimer.CurrentTime)
                {
                    var startTime = _handler.LastSimulationTime;
                    var stepTime = _simulationTimer.StepTime;
                    _handler.LastSimulationTime += stepTime;
                    _lastStep = _handler.LastSimulationTime >= _simulationTimer.CurrentTime;

                    _logger.DebugFormat("Simulation Time is {0}", _handler.LastSimulationTime);
                    executeCount++;

                    ExecuteSubSystems(Stage.UpdateSimulationTime, "UpdateSimulationTime",
                        CustomProfilerStep.VehicleUpdateSimulationTime);

                    if (_firstStep)
                    {
                        ExecuteSubSystems(Stage.SyncFromComponent, "SyncFromComponent",
                            CustomProfilerStep.VehicleSyncFromComponent);
                        ExecuteCmdsInSubSystems(Stage.ExecuteCmds, startTime, _handler.LastSimulationTime, "ExecuteCmds",
                            CustomProfilerStep.VehicleExecuteCmds);
                    }
                   
                    ExecuteSubSystems(Stage.FixedUpdate, "FixedUpdate", CustomProfilerStep.VehicleFixedUpdate);

                    Simulate("Simulation", CustomProfilerStep.VehicleSimulation);

                    if (_lastStep)
                    {
                        ExecuteSubSystems(Stage.Update, "Update", CustomProfilerStep.VehicleUpdate);
                        ExecuteSubSystems(Stage.SyncToComponent, "SyncToComponent", CustomProfilerStep.VehicleSyncToComponent);
                    }
                   
                    _firstStep = false;
                }

                SingletonManager.Get<DurationHelp>().ProfileAddExecuteCount(CustomProfilerStep.VehicleSimulation, executeCount);

                if (_simulationTimer.CurrentTime > 0)
                {
                    AssertUtility.Assert(_handler.LastSimulationTime == _simulationTimer.CurrentTime);
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("{0}", e);
            }
            finally
            { 
                _vehicleSelector.LateUpdate();
                var elapsed = StopProfiler(CustomProfilerStep.VechiclePrediction);
                SingletonManager.Get<DurationHelp>().ProfileAddInfo(CustomProfilerStep.VechiclePrediction, elapsed.ToString("F3"));
            }
        }

        private void Simulate(string stageName, CustomProfilerStep step)
        {
            StartProfilers(stageName, step);
            PhysicsUtility.Simulate(_simulationTimer.SimulationStepTime);
            StopProfilers(stageName, step);
        }

        private void ExecuteSubSystems(Stage stage, string stageName, CustomProfilerStep step)
        {
            try
            {
                StartProfilers(stageName, step);

                _stage = stage;

                var vehicleEntityList = _vehicleSelector.ActiveVehicles;
                int vehicleCount = vehicleEntityList.Count;
                for(int i = 0; i < vehicleCount; ++i)
                {
                    var entity = vehicleEntityList[i];
                    _currentEntity = entity;
  
                    ExecuteVehicleSystems();
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception {0}", e);
            }
            finally
            {
                StopProfilers(stageName, step);
            }
          
        }

        private void ExecuteVehicleSystems()
        {
            for (int i = 0; i < Systems.Count; i++)
            {
                var module = Systems[i];
                try
                {
                    if(module.IsEntityValid(_currentEntity))
                        SingleExecute(module);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("error executing {0}: {1}", module.GetType(), e);
                }
            }
        }

        private void ExecuteCmdsInSubSystems(Stage stage, int startTime, int endTime, string stageName, CustomProfilerStep step)
        {
            try
            {
                StartProfilers(stageName, step);

                _stage = stage;
                if (_isServer && !_serverAuthorative)
                {
                    ExecuteLatestCmdsInSubSystems();
                }
                else
                {
                    ExecuteCmdsInSubSystemsBetween(startTime, endTime);
                }
            }
            catch (Exception e)

            {
                _logger.ErrorFormat("Exception {0}", e);
            }
            finally
            {
                StopProfilers(stageName, step);
            }
            

            
        }

#pragma warning disable RefCounter001
        private void ExecuteLatestCmdsInSubSystems()
        {
            foreach (IVehicleCmdOwner owner in _handler.VehicleCmdOwnerList)
            {
                _currentEntity = owner.Entity;
                var userCmd = owner.LatestCmd;
                if (userCmd == null)
                {
                    continue;
                }
                

                _logger.DebugFormat("execute vehicle latest cmd  at time {0} on vehicle {1}", userCmd.ExecuteTime, userCmd.VehicleId);

                _currentCmd = userCmd;
                ExecuteVehicleSystems();
                owner.ClearCmdList();
            }
        }
#pragma warning restore RefCounter001

        private void ExecuteCmdsInSubSystemsBetween(int startTime, int endTime)
        {
            
            foreach (IVehicleCmdOwner owner in _handler.VehicleCmdOwnerList)
            {
                _currentEntity =  owner.Entity;
                foreach (var userCmd in owner.GetCmdList(startTime, endTime))
                {
                    _currentCmd = userCmd;
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.DebugFormat("execute vehicle simulation {0}, at time {1} on vehicle {2}", startTime, userCmd.ExecuteTime, userCmd.VehicleId);
                    }

                    _logger.DebugFormat("Simulation Cmd is {0} seq {1} move {2} {3}", userCmd.ExecuteTime, userCmd.CmdSeq, userCmd.MoveHorizontal, userCmd.MoveVertical);
                    ExecuteVehicleSystems();
                }
            }
        }

        private void StartProfiler(CustomProfilerStep step)
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(step);
        }

        private float StopProfiler(CustomProfilerStep step)
        {
            if (!_lastStep)
            {
                SingletonManager.Get<DurationHelp>().ProfilePause(step);
                return 0.0f;
            }
            else
            {
               return SingletonManager.Get<DurationHelp>().ProfileEnd(step);
            }
        }

        private void StartProfiler(string name)
        {
            _profiler.Start(name);
        }

        private void StopProfiler(string name)
        {
            if (!_lastStep)
            {
                _profiler.Pause(name);
            }
            else
            {
                _profiler.Stop(name);
            }
        }

        private void StartProfilers(string name, CustomProfilerStep step)
        {
            StartProfiler(step);
#if UNITY_EDITOR
            StartProfiler(name);
#endif
        }

        private void StopProfilers(string name, CustomProfilerStep step)
        {
#if UNITY_EDITOR
            StopProfiler(name);
#endif
            StopProfiler(step);

        }
    }
}