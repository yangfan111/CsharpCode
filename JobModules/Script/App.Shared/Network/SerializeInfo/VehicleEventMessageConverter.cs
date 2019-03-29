using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Core.EntityComponent;
using Core.Prediction.VehiclePrediction.Event;

namespace App.Shared.Network
{
    public class VehicleEventMessageConverter 
    {
        public static IVehicleSyncEvent FromProtoBuf(VehicleEventMessage message)
        {
            switch (message.EventType)
            {
                case (int) VehicleSyncEventType.Damage:
                {
                    var damageSyncEvent = VehicleDamangeSyncEvent.Allocate();
                    damageSyncEvent.EType = VehicleSyncEventType.Damage;
                    damageSyncEvent.SourceObjectId = message.SourceId;
                    damageSyncEvent.TargetObject = new EntityKey(message.Ints[1], (short)message.Ints[0]);
                    damageSyncEvent.Damage = message.Floats[0];
                    return damageSyncEvent;
                }
                default:
                    throw new Exception("Unkown VehicleSyncEventType : " + message.EventType);
            }
        }

        public static void ToProtoBuf(VehicleEventMessage msg, IVehicleSyncEvent syncEvent)
        {
            switch (syncEvent.EType)
            {
                case VehicleSyncEventType.Damage:
                {
                    msg.EventType = (int) syncEvent.EType;
                    msg.SourceId = syncEvent.SourceObjectId;
                    var damageSyncEvent = (VehicleDamangeSyncEvent) syncEvent;
                    msg.Ints.Add(damageSyncEvent.TargetObject.EntityType);
                    msg.Ints.Add(damageSyncEvent.TargetObject.EntityId);
                    msg.Floats.Add(damageSyncEvent.Damage);
                    break;
                }
                default:
                    throw new Exception("Unkown VehicleSyncEvent Type : " + syncEvent.EType);
            }

        }
    }
}
