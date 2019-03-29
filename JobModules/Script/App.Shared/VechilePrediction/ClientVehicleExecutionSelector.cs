using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.Configuration;
using Core.Prediction.VehiclePrediction;
using Entitas;
using Enum = Google.Protobuf.WellKnownTypes.Enum;

namespace App.Shared.VechilePrediction
{

    public class ClientVehicleExecutionSelector : VehicleExecutionSelector
    {

        private static readonly float LodDistanceDamper = 0.95f;

        private bool _isPredictMode;
        private float _sqrLodDistance;
        public ClientVehicleExecutionSelector(Contexts contexts) : base(contexts)
        {
            _isPredictMode = SharedConfig.IsOffline || SharedConfig.DynamicPrediction;
            _sqrLodDistance = 1000.0f * 1000.0f;
        }

        protected override void UpdateVehicles()
        {
            //enable/disable the gameobject according to distance
            var selfEntity = PlayerContext.flagSelfEntity;
            var vehicleEntities = Vehicles.GetEntities();
            var vehicleCount = vehicleEntities.Length;
            var lodCullDistance = _sqrLodDistance;
            var physicsCullDistance = SqrPhysicsDistance;
            if (SharedConfig.DisableVehicleCull)
            {
                lodCullDistance = physicsCullDistance = float.MaxValue;
            }

            for (int i = 0; i < vehicleCount; ++i)
            {
                var vehicle = vehicleEntities[i];
                var distance = SqrDistance(selfEntity, vehicle);

                var active = false;
                var lowLod = true;
                if(distance < lodCullDistance * LodDistanceDamper)
                {
                    active = true;
                    if (distance < physicsCullDistance * PhysicsDistanceDamper)
                        lowLod = false;
                    else if (distance < physicsCullDistance)
                        lowLod = vehicle.IsLowLod();
                }
                else if(distance < lodCullDistance)
                {
                    active = vehicle.IsActiveSelf();
                    lowLod = vehicle.IsLowLod();
                }

                SetActiveDelay(new ActiveSetting(vehicle, active, lowLod));
            }
        }

        protected override void SetActive(ActiveSetting activeSetting)
        {
            var vehicle = activeSetting.Vehicle;
            var active = activeSetting.Active;
            vehicle.SetActive(active);
            if (active)
            {
                if (_isPredictMode)
                    NotifyVehicleActive(vehicle);
                else
                {
                    var selfEntity = PlayerContext.flagSelfEntity;

                    if (selfEntity.IsOnVehicle() && 
                        vehicle == VehicleContext.GetEntityWithEntityKey(selfEntity.controlledVehicle.EntityKey))
                    {
                        NotifyVehicleActive(vehicle);
                    }
                }

                vehicle.SetLodLevel(activeSetting.LowLod);
            }
        }
    }
}
