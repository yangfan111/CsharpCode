using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Shared;
using Core.EntityComponent;
using Core.Network;
using Core.Prediction.VehiclePrediction.Event;
using Core.Utils;

namespace App.Server.MessageHandler
{
    internal class VehicleEventMessageHandler : AbstractServerMessageHandler<PlayerEntity, IVehicleSyncEvent>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleEventMessageHandler));

        private Contexts _contexts;
        public VehicleEventMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            _contexts = contexts;
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, IVehicleSyncEvent messageBody)
        {
            var sourceKey = new EntityKey(messageBody.SourceObjectId, (short) EEntityType.Vehicle);
            var vehicle = _contexts.vehicle.GetEntityWithEntityKey(sourceKey);
            if (vehicle != null)
            {
                vehicle.vehicleSyncEvent.SyncEvents.Enqueue(messageBody);
                messageBody.AcquireReference();
            }
            else
            {
                _logger.InfoFormat("Can not found vehicle {0} for vehicle sync event {1}", sourceKey.EntityId, messageBody.EType);
               
            }
        }
    }
}
