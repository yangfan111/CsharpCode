using System.Collections.Generic;
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

        private LinkedList<VehicleEntity> _cachedVehicleEntity;
        private bool _isLoading;

        public ClientVehicleEntityInitSystem(Contexts contexts) : base(contexts.vehicle)
        {
            _contexts = contexts;
            _cachedVehicleEntity = new LinkedList<VehicleEntity>();
            _isLoading = false;
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
            _cachedVehicleEntity.AddLast(vehicle);
            LoadVehicle();
        }

        public void OnLoadSucc(VehicleEntity vehicle, UnityObject unityObj)
        {

            InitLoadedVehicle(vehicle, unityObj);
            //load next vehicle
            _isLoading = false;
            LoadVehicle();
        }

        private void LoadVehicle()
        {
            if (!_isLoading && _cachedVehicleEntity.Count > 0)
            {
                var vehicle = _cachedVehicleEntity.First.Value;
                _cachedVehicleEntity.RemoveFirst();

                var assetBundleName = vehicle.vehicleAssetInfo.AssetBundleName;
                var modelName = vehicle.vehicleAssetInfo.ModelName;
                var position = Vector3.zero;
                var rotation = Quaternion.identity;
                if (!vehicle.hasVehicleType)
                {
                    vehicle.AddVehicleType((EVehicleType)vehicle.vehicleAssetInfo.VType);
                }

                if (vehicle.HasDynamicData())
                {
                    var dataComp = vehicle.GetDynamicData();
                    rotation = dataComp.Rotation;
                    position = dataComp.Position.ShiftedVector3();
                }
                AssetManager.LoadAssetAsync(vehicle, AssetConfig.GetVehicleAssetInfo(assetBundleName, modelName), OnLoadSucc, new AssetLoadOption(position:position, rotation: rotation));
                _logger.InfoFormat("created client vehicle entity {0}", vehicle.entityKey.Value);
                _isLoading = true;
            }
        }

        private void InitLoadedVehicle(VehicleEntity vehicle, UnityObject unityObj)
        {
            var go = unityObj.AsGameObject;
            AssertUtility.Assert(go != null);

            //vehicle.gameObject.Value.GetComponent<Rigidbody>().isConstantColliderPose = true;

            _logger.InfoFormat("Client Create Vehicle {0} Enter OnLoadSucc  EntityKey {1} Position {2}", unityObj.Address, vehicle.entityKey, go.transform.position);

            // UnityEngine.Debug.LogError("Client Load Vehicle Successfully!!!");
            if (vehicle.HasDynamicData())
            {
                var dataComp = vehicle.GetDynamicData();
                go.transform.rotation = dataComp.Rotation;
                go.transform.position = dataComp.Position.ShiftedVector3();
            }

            _logger.InfoFormat("Client Create Vehicle {0} Transform Set  EntityKey {1} Position {2}", unityObj.Address, vehicle.entityKey, go.transform.position);

            vehicle.AddVehicleComponentsPostInit((EVehicleType)vehicle.vehicleAssetInfo.VType, unityObj,
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