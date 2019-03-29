using System.Collections;
using System.Collections.Generic;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Components.Vehicle;
using App.Shared.DebugHandle;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Vehicle;
using Utils.AssetManager;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Room;
using UnityEngine;
using Core.Configuration;
using Core.Utils;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Utils.Configuration;
using XmlConfig;
using App.Shared.GameInputFilter;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Offline
{
    public class OfflineGamePlay : IModuleInitSystem, IEntityInitSystem, ILateUpdateSystem
    {

        private ICommonSessionObjects _sessionObjects;


        public OfflineGamePlay(Contexts contexts, ICommonSessionObjects sessionObjects)
        {
            _contexts = contexts;
            _sessionObjects = sessionObjects;
        }

        private Contexts _contexts;
      
        public void OnInitModule(IUnityAssetManager assetManager)
        {
            var sessionObjects = _contexts.session.commonSession;
            IEntityIdGenerator idGenerator = sessionObjects.EntityIdGenerator;
       
          
//            IPlayerInfo aiInfo = new PlayerInfo(TestUtility.TestToken, 1, "AI", TestUtility.PlayerSex);
//            PlayerEntityFactory.CreateNewPlayerEntity(_contexts.player, idGenerator.GetNextEntityId(), 0, SharedConfig.modelName,SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition, aiInfo, false, true);
// 
            CreateVehicles(idGenerator);
//            CreateVehicles(EVehicleType.Ship, idGenerator);

           
            ((OfflineSnapshotSelector)_contexts.session.clientSessionObjects.SnapshotSelectorContainer.SnapshotSelector).Init();
            PhysicsUtility.SetAutoSimulation(false);
            CreateSceneObject();
            
        }
		
		public void OnEntityInit()
        {
          //  ((OfflineSnapshotSelector)_contexts.session.clientSessionObjects.SnapshotSelectorContainer.SnapshotSelector).Init();
        }

        public void CreateVehicles(IEntityIdGenerator idGenerator)
        {
            var vehicles = VehicleEntityUtility.CreateVehicles(_contexts.session.commonSession.RoomInfo.MapId, _contexts.vehicle, idGenerator);
            foreach (var vehicle in vehicles)
            {
                vehicle.isFlagOffline = true;
            }

        }

        public void CreateSceneObject()
        {
            //GenerateWeapons();
            //GenerateGameItems();
        }

        private void GenerateGameItems()
        {
            var config = SingletonManager.Get<GameItemConfigManager>().GetConfig();
            var list = new System.Collections.Generic.List<int>();
            foreach (var item in config.Items)
            {
                list.Add(item.Id);
            }
            var r = new System.Random();
            var basePos = SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition;
            int sceneObjectCount = list.Count;
            for (int i = 0; i < sceneObjectCount; i++)
            {
                var id = list[i % list.Count];
                var count = 1;
                var time = 1;
                while (time-- > 0)
                {
                   _contexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity(
                        ECategory.GameItem,
                        id,
                        count,
                        RaycastUtility.GetLegalPosition(new Vector3(basePos.x + r.Next(-10, 10), basePos.y, basePos.z + r.Next(-10, 10))));
                }
            }
        }

        private void GenerateWeapons()
        {
            var items = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigs();
            var list = new System.Collections.Generic.List<int>();
            foreach (var item in items)
            {
                list.Add(item.Key);
            }
            var r = new System.Random();
            var basePos = SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition;
            int sceneObjectCount = list.Count;
            for (int i = 0; i < sceneObjectCount; i++)
            {
                var id = list[i % list.Count];
                var count = 1;
                var time = 1;
                while (time-- > 0)
                {
                   _contexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity(
                        ECategory.Weapon,
                        id,
                        count,
                        RaycastUtility.GetLegalPosition(new Vector3(basePos.x + r.Next(-1, 1), basePos.y, basePos.z + r.Next(-1, 1))));
                }
            }
        }

        public void OnLateUpdate()
        {
            SingletonManager.Get<VehicleDebugCommonData>().ExecuteCreateVehicle(_contexts);
        }
    }
}