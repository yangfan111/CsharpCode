using App.Shared.GameModules.Vehicle;
using App.Shared.Player;
using Core.Prediction.VehiclePrediction;
using Core.Utils;
using Entitas;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Utils.Singleton;

namespace App.Shared.VechilePrediction
{
    public class VehicleExecutionSelector : IVehicleExecutionSelector
    {
        protected struct ActiveSetting
        {
            public VehicleEntity Vehicle;
            public bool Active;
            public bool LowLod;

            public ActiveSetting(VehicleEntity vehicle, bool active)
            {
                Vehicle = vehicle;
                Active = active;
                LowLod = false;
            }

            public ActiveSetting(VehicleEntity vehicle, bool active, bool lowlod)
            {
                Vehicle = vehicle;
                Active = active;
                LowLod = lowlod;
            }
        }

        protected VehicleContext VehicleContext;
        protected PlayerContext PlayerContext;
        protected IGroup<VehicleEntity> Vehicles;
        private IGroup<PlayerEntity> _players;
        private int _activeCacheIndex = 0;
        private List<Entity>[] _activeVehicleCaches;
        private bool _ignorePlayerVehicleCollision;
        protected float SqrPhysicsDistance;

        private Queue<ActiveSetting> _activeWaitingQueue = new Queue<ActiveSetting>();

        protected static  readonly float PhysicsDistanceDamper = 0.81f;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleExecutionSelector));
        public VehicleExecutionSelector(Contexts contexts)
        {
            VehicleContext = contexts.vehicle;
            PlayerContext = contexts.player;

            _activeVehicleCaches = new List<Entity>[2];
            _activeVehicleCaches[0] = new List<Entity>();
            _activeVehicleCaches[1] = new List<Entity>();

            Vehicles = VehicleContext.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.GameObject, VehicleMatcher.Position));
            _players = PlayerContext.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.ThirdPersonModel, PlayerMatcher.GamePlay));
            _ignorePlayerVehicleCollision = false;
            SqrPhysicsDistance = 150.0f * 150.0f;

        }

        protected float SqrDistance(PlayerEntity player, VehicleEntity vehicle)
        {
            var playerPosition = player.position.Value;
            var vehiclePosition = vehicle.position.Value;
            var x = playerPosition.x - vehiclePosition.x;
            var z = playerPosition.z - vehiclePosition.z;
            return x * x + z * z;
        }

        public void Update()
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.VehicleEntityUpdate);
            if (CanUpdate())
            {
                NextActiveVehicles.Clear();
                UpdateVehicles();
            }
            
            SetVehicleStates();
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.VehicleEntityUpdate);
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.VehiclePlayerLayerSet);
            UpdatePlayers();
            SingletonManager.Get<DurationHelp>().ProfilePause(CustomProfilerStep.VehiclePlayerLayerSet);
        }

        private void ChangeActiveCache()
        {
            _activeCacheIndex = _activeCacheIndex == 0 ? 1 : 0;
        }

        public void LateUpdate()
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.VehiclePlayerLayerSet);
            LateUpdatePlayers();
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.VehiclePlayerLayerSet);
        }

        private void SetVehicleStates()
        {
            int updateCount = SharedConfig.VehicleActiveUpdateRate;
            while (_activeWaitingQueue.Count > 0 && updateCount > 0)
            {
                var activeSetting = _activeWaitingQueue.Dequeue();
                SetActive(activeSetting);
                updateCount--;

                if (_activeWaitingQueue.Count == 0)
                {
                    ChangeActiveCache();
                }
            }
        }

        protected virtual void UpdateVehicles()
        {
            var players = PlayerContext.GetEntities();
            var vehicles = Vehicles.GetEntities();
            int pc = players.Length;
            int vc = vehicles.Length;
            var cullDistance = SharedConfig.DisableVehicleCull ? float.MaxValue : SqrPhysicsDistance;
            for (int vi = 0; vi < vc; vi++)
            {
                var v = vehicles[vi];
                bool isPlayerNear = false;
                bool isPlayerFar = true;
                for (int pi = 0; pi < pc; pi++)
                {
                    var p = players[pi];
                    var sqrDistance = SqrDistance(p, v);
                    if (sqrDistance < cullDistance * PhysicsDistanceDamper)
                    {
                        isPlayerNear = true;
                        break;
                    }

                    if (sqrDistance < cullDistance)
                    {
                        isPlayerFar = false;
                    }
                }

                bool active = isPlayerNear || !isPlayerFar && v.IsActiveSelf();

                SetActiveDelay(new ActiveSetting(v, active));
            }
        }

        protected void SetActiveDelay(ActiveSetting activeSetting)
        {
            _activeWaitingQueue.Enqueue(activeSetting);
        }

        private bool CanUpdate()
        {
            return _activeWaitingQueue.Count == 0;
        }

        protected  virtual void SetActive(ActiveSetting activeSetting)
        {
            var vehicle = activeSetting.Vehicle;
            var active = activeSetting.Active;
            vehicle.SetActive(active);
            if (active)
            {
                NotifyVehicleActive(vehicle);
            }
        }

        public IList<Entity> ActiveVehicles {get { return _activeVehicleCaches[_activeCacheIndex]; }}

        private IList<Entity> NextActiveVehicles
        {
            get
            {
                var nextActiveCacheIndex = _activeCacheIndex == 0 ? 1 : 0;

                return _activeVehicleCaches[nextActiveCacheIndex];
            }
        }

        protected void NotifyVehicleActive(VehicleEntity vehicle)
        {

            NextActiveVehicles.Add(vehicle);
        }

        private void UpdatePlayers()
        {
            IgnorePlayerVehicleCollision(true);
            _ignorePlayerVehicleCollision = true;
        }

        private void LateUpdatePlayers()
        {
            if (_ignorePlayerVehicleCollision)
            {
                IgnorePlayerVehicleCollision(false);
            }
        }

        private void IgnorePlayerVehicleCollision(bool ignore)
        {
            var layer = ignore ? UnityLayerManager.GetLayerIndex(EUnityLayerName.PlayerIgnoreVehicle) : UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
            var players = _players.GetEntities();
            int playerCount = players.Length;
            for (int i = 0; i < playerCount; ++i)
            {
                try
                {
                    var player = players[i];
                    var gamePlayer = player.gamePlay;
                    var root = player.RootGo();
                    if (root == null)
                    {
                        _logger.ErrorFormat("GameObject for Player Entity {0} Deleted Destroy Flag {1}", player.hasEntityKey ? player.entityKey.ToString() : "NoEntityKey", player.isFlagDestroy);
                        continue;
                    }

                    if (gamePlayer.IsDead() || !root.activeSelf)
                    {
                        continue;
                    }

                    root.layer = layer;
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("{0}", e);
                }
                
            }
        }

        public int ActiveCount
        {
            get { return _activeVehicleCaches[_activeCacheIndex].Count; }
        }

        public PlayerContext GetPlayerContext()
        {
            return PlayerContext;
        }
    }
}
