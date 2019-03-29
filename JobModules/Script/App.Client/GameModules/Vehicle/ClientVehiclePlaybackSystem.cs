using System;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Components;
using Core.GameModule.Interface;
using App.Shared.Components.ClientSession;
using App.Shared.DebugHandle;
using App.Shared.GameModules.Vehicle;
using App.Shared.Network;
using App.Shared.UserPhysics;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;
using EVP;
using EVP.Scripts;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Vehicle
{
    public class ClientVehiclePlaybackSystem : IPlaybackSystem, IOnGuiSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientVehiclePlaybackSystem));

        private IGroup<VehicleEntity> _vehicles;
        private PlayerContext _playerContext;
        private VehicleContext _vehicleContext;
        public ClientVehiclePlaybackSystem(PlayerContext playerContext, VehicleContext vehicleContext)
        {
            _vehicleContext = vehicleContext;
            _playerContext = playerContext;
            _vehicles = vehicleContext.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.GameObject));
        }


        public void OnPlayback()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.VehiclePlayBack);
                var selfPlayer = _playerContext.flagSelfEntity;
                var selfVehicle = PlayerVehicleUtility.GetControlledVehicle(selfPlayer, _vehicleContext);

                if (SharedConfig.DynamicPrediction)
                {
                    if (selfVehicle != null)
                    {
                        VehicleStateUtility.SyncOnPlayerRideOn(selfVehicle);
                    }
                }
                else
                {
                    var vehicles = _vehicles.GetEntities();
                    for (int i = 0; i < vehicles.Length; ++i)
                    {
                        var vehicle = vehicles[i];
                        if (vehicle != selfVehicle && !vehicle.isFlagOffline && VehicleStateUtility.IsReadyForSync(vehicle))
                        {
                            VehicleStateUtility.SyncFromComponent(vehicle);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception {0}", e);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.VehiclePlayBack);
            }
            
        }

        private StringBuilder _guiInfo = new StringBuilder();
        private GUIStyle _lableStyle = new GUIStyle();
        public void OnGUI()
        {
            if (Camera.main == null || !VehicleDebugUtility.ShowDebugInfo)
            {
                return;
            }

            _lableStyle.normal.textColor = Color.cyan;
            var vehicles = _vehicles.GetEntities();
            for (int i = 0; i < vehicles.Length; ++i)
            {
                var vehicle = vehicles[i];
                if (!vehicle.hasEntityKey || !vehicle.IsActiveSelf())
                {
                    continue;
                }

                var id = vehicle.entityKey.Value.EntityId;
                if (SharedConfig.IsOffline)
                {
                    id -= EntityIdGenerator.LocalBaseId;
                }

                vehicle.BuildVehicleGUIInfo(id, _guiInfo);
                var pos = Camera.main.WorldToScreenPoint(vehicle.gameObject.UnityObject.AsGameObject.transform.position + new Vector3(0, 1.8f, 0));
                
                if(pos.z > 0)
                    GUI.Label(new Rect(pos.x, Screen.height - pos.y, 400f, 20f), _guiInfo.ToString(), _lableStyle);
            }
        }
    }
}
