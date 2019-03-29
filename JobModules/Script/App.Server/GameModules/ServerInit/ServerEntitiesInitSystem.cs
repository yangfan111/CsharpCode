using System.Collections;
using App.Shared.Components.Vehicle;
using System.Collections.Generic;
using App.Shared.GameModules.Vehicle;
using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;
using Core;
using App.Shared.Configuration;
using Core.Configuration;

namespace App.Server
{
    public class ServerEntitiesInitSystem : IModuleInitSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerEntitiesInitSystem));
        private Contexts _contexts;

        public ServerEntitiesInitSystem(Contexts contexts)
        {
            _contexts = contexts;
        }



        public void OnInitModule(IUnityAssetManager assetManager)
        {
            //CreateVehicles();
        }

        public void CreateVehicles()
        {
            VehicleEntityUtility.CreateVehicles(_contexts.session.commonSession.RoomInfo.MapId, _contexts.vehicle, _contexts.session.commonSession.EntityIdGenerator);
        }
    }
}