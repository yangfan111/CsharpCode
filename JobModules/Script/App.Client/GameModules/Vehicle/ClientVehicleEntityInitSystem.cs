using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.GameModules.Vehicle;
using Utils.AssetManager;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;
using XmlConfig;

namespace App.Client.GameModules.Vehicle
{
    public class ClientVehicleEntityInitSystem : ReactiveResourceLoadSystem<VehicleEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientVehicleEntityInitSystem));

        private Contexts _contexts;

        public ClientVehicleEntityInitSystem(Contexts contexts) : base(contexts.vehicle)
        {
            _contexts = contexts;
        }

        protected override ICollector<VehicleEntity> GetTrigger(IContext<VehicleEntity> context)
        {
            return context.CreateCollector(VehicleMatcher.VehicleAssetInfo.Added());
        }

        protected override bool Filter(VehicleEntity entity)
        {
            return !entity.hasGameObject && entity.hasVehicleAssetInfo;
        }

        public override void SingleExecute(VehicleEntity vehicle)
        {
            var assetBundleName = vehicle.vehicleAssetInfo.AssetBundleName;
            var modelName = vehicle.vehicleAssetInfo.ModelName;
            AssetManager.LoadAssetAsync(vehicle, AssetConfig.GetVehicleAssetInfo(assetBundleName, modelName), OnLoadSucc);
            _logger.InfoFormat("created client vehicle entity {0}", vehicle.entityKey.Value);
        }


        public void OnLoadSucc(VehicleEntity vehicle, UnityObject unityObj)
        {
            var go = unityObj.AsGameObject;
            AssertUtility.Assert(go != null);

            //vehicle.gameObject.Value.GetComponent<Rigidbody>().isConstantColliderPose = true;
            if (!vehicle.hasVehicleType)
            {
                vehicle.AddVehicleType((EVehicleType) vehicle.vehicleAssetInfo.VType);
            }

            // UnityEngine.Debug.LogError("Client Load Vehicle Successfully!!!");
            if (vehicle.HasDynamicData())
            {
                var dataComp = vehicle.GetDynamicData();
                go.transform.rotation = dataComp.Rotation;
                go.transform.position = dataComp.Position;
            }
            else
            {
                go.transform.eulerAngles = new Vector3();
                go.transform.position = new Vector3(11, 50, 17);
            }

            vehicle.AddVehicleComponentsPostInit((EVehicleType) vehicle.vehicleAssetInfo.VType, unityObj,
                _contexts.player, false);
            var vehicleTimer = _contexts.session.clientSessionObjects.VehicleTimer;
            vehicle.SetTimer(vehicleTimer);

            if (!vehicle.isFlagOffline && !SharedConfig.DynamicPrediction)
            {
                vehicle.SetKinematic(true);
            }

            if (SharedConfig.DynamicPrediction)
            {
                VehicleStateUtility.SetVehicleSyncLatest(vehicle, true);
            }

            var target = RayCastTargetUtil.AddRayCastTarget(go);
            VehicleCastData.Make(target, vehicle.entityKey.Value.EntityId);
        }
    }
}