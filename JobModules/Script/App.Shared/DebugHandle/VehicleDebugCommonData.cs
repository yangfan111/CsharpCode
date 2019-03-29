using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.DebugHandle
{
    public class VehicleDebugCommonData : DisposableSingleton<VehicleDebugCommonData>
    {
        private struct VehicleCreateRequest
        {
            public Vector3 Position;
            public int VehicleId;
            public int Count;

            public VehicleCreateRequest(Vector3 position, int vehicleId, int count)
            {
                Position = position;
                VehicleId = vehicleId;
                Count = count;
            }
        }

        private  Queue<VehicleCreateRequest> _vehicleCreateRequests = new Queue<VehicleCreateRequest>();

        public void CreateVehicle(Vector3 position, int vehicleId, int count)
        {
            _vehicleCreateRequests.Enqueue(new VehicleCreateRequest(position, vehicleId, count));
        }

        public void ExecuteCreateVehicle(Contexts contexts)
        {
            if (_vehicleCreateRequests.Count > 0)
            {
                var req = _vehicleCreateRequests.Dequeue();
                var idGenerator = contexts.session.commonSession.EntityIdGenerator;

                for (int i = 0; i < req.Count; ++i)
                {
                    var id = idGenerator.GetNextEntityId();
                    VehicleEntityUtility.CreateNewVehicle(contexts.vehicle, req.VehicleId, id, req.Position);
                }
                   
            }
        }
    

        protected override void OnDispose()
        {
            _vehicleCreateRequests.Clear();
        }
    }
}
