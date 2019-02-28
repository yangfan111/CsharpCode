using System.Collections.Generic;
using App.Shared;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.UserPhysics;
using Core.GameModule.System;
using Entitas;
using UnityEngine;
using Utils.AssetManager;
using System;
using XmlConfig;

namespace App.Server.GameModules.Vehicle
{
    class ServerVehicleEntityInitSystem : ReactiveResourceLoadSystem<VehicleEntity>
    {
        private Contexts _contexts;
        private VehicleLoadResponseHandler _loadResourceHandler;

        public ServerVehicleEntityInitSystem(Contexts contexts) : base(contexts.vehicle)
        {
            _contexts = contexts;
            _loadResourceHandler = new VehicleLoadResponseHandler(contexts);
        }

        protected override ICollector<VehicleEntity> GetTrigger(IContext<VehicleEntity> context)
        {
            return context.CreateCollector(VehicleMatcher.EntityKey.Added());
        }

        protected override bool Filter(VehicleEntity entity)
        {
            return true;
        }

        public override void SingleExecute(VehicleEntity vehicle)
        {
            var assetBundleName = vehicle.vehicleAssetInfo.AssetBundleName;
            var modleName = vehicle.vehicleAssetInfo.ModelName;
            var assetInfo = AssetConfig.GetVehicleAssetInfo(assetBundleName, modleName);
            AssetManager.LoadAssetAsync(vehicle, assetInfo, _loadResourceHandler.OnLoadSucc);
        }

       

        class VehicleLoadResponseHandler
        {
            private Contexts _contexts;

            public VehicleLoadResponseHandler(Contexts contexts)
            {
                _contexts = contexts;
            }

            public void OnLoadSucc(VehicleEntity vehicle, UnityObject unityObj)
            {
                var vehicleTimer = _contexts.session.serverSessionObjects.VehicleTimer; ;

                var data = vehicle.GetDynamicData();

                var go = unityObj.AsGameObject;
                go.transform.position = data.Position;
                go.transform.rotation = data.Rotation;

                vehicle.AddVehicleComponentsPostInit((EVehicleType) vehicle.vehicleAssetInfo.VType, unityObj, _contexts.player, true);
                
                vehicle.SetTimer(vehicleTimer);
            }
        }
    }
}
