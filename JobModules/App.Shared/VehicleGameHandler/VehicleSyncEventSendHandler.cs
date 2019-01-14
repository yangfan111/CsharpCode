using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using Core.Network;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleSyncEventSendHandler : VehicleUpdateHandler
    {
        private ClientSessionObjectsComponent _sessionObjects;
        private bool _isOffline;
        public VehicleSyncEventSendHandler(ClientSessionObjectsComponent sessionObjects, bool isOffline)
        {
            _sessionObjects = sessionObjects;
            _isOffline = isOffline;
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            if (_isOffline)
            {
                var syncEvents = vehicle.vehicleSyncEvent.SyncEvents;
                while (syncEvents.Count > 0)
                {
                    var e = syncEvents.Dequeue();
                    e.ReleaseReference();
                }
            }
            else
            {
                var channel = _sessionObjects.NetworkChannel;
                if (channel != null)
                {
                    var syncEvents = vehicle.vehicleSyncEvent.SyncEvents;
                    while (syncEvents.Count > 0)
                    {
                        var e = syncEvents.Dequeue();
                        channel.SendReliable((int) EClient2ServerMessage.VehicleEvent, e);
                        e.ReleaseReference();
                    }
                }
            }
            

            
        }
    }
}
