using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.Event;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleSyncEventReceiveHandler : VehicleUpdateHandler
    {
        private Contexts _contexts;
        public VehicleSyncEventReceiveHandler(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            var syncEvents = vehicle.vehicleSyncEvent.SyncEvents;
            while(syncEvents.Count > 0)
            {
                var e = syncEvents.Dequeue();
                switch (e.EType)
                {
                    case VehicleSyncEventType.Damage:
                        ProcessDamageSyncEvent((VehicleDamangeSyncEvent) e);
                        break;
                }

                e.ReleaseReference();
            }
        }

        private void ProcessDamageSyncEvent(VehicleDamangeSyncEvent e)
        {
            switch (e.TargetObject.EntityType)
            {
                case (int) EEntityType.Player:
                    var player = _contexts.player.GetEntityWithEntityKey(e.TargetObject);
                    if (player != null)
                    {
                        DoProcessDamageToPlayer(_contexts, player, e.Damage);
                    }
                    break;
                case (int) EEntityType.Vehicle:
                    var vehicle = _contexts.vehicle.GetEntityWithEntityKey(e.TargetObject);
                    if (vehicle != null)
                    {
                        DoProcessDamageToVehicle(vehicle, e.Damage);
                    }
                    break;
            }
            
        }

        private void DoProcessDamageToPlayer(Contexts contexts, PlayerEntity player, float damage)
        {
            VehicleDamageUtility.DoPlayerDamage(contexts, null, player, damage);
        }

        private void DoProcessDamageToVehicle(VehicleEntity vehicle, float damage)
        {
            vehicle.GetGameData().DecreaseHp(VehiclePartIndex.Body, damage);
        }
    }
}
