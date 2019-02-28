using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using App.Shared.Terrains;
using Core.SessionState;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Terrain
{
    public class TerrainTestSystem : AbstractStepExecuteSystem
    {
        private Contexts _contexts;
        private IMyTerrain _myTerrain;

        public static string mapName = "";
        public static Vector3 pos = new Vector3();
        public static Vector3 velocity = new Vector3();
        public static float gripFriction;
        public static float dragFriction;
        public static int textureId;
        public static float yaw;
        public static int mark;

        public static int teamCnt;
        public static string teamNum;

        public TerrainTestSystem(Contexts contexts)
        {
            _contexts = contexts;
            _myTerrain = SingletonManager.Get<TerrainManager>().GetTerrain(SingletonManager.Get<MapConfigManager>().SceneParameters.Id);
            mapName = SingletonManager.Get<MapConfigManager>().SceneParameters.MapName;
        }

        protected override void InternalExecute()
        {
       
            PlayerEntity playerEntity = _contexts.player.flagSelfEntity;
            if (_myTerrain != null && playerEntity != null)
            {
                Vector3 playerPos = playerEntity.position.Value;

                pos.Set(playerPos.x - _myTerrain.InitPosition.x, playerPos.y - _myTerrain.InitPosition.y, playerPos.z - _myTerrain.InitPosition.z);

                velocity = playerEntity.playerMove.Velocity;
                if (playerEntity.IsOnVehicle())
                {
                    var vehicle = PlayerVehicleUtility.GetVehicle(playerEntity, _contexts.vehicle);
                    if (vehicle != null)
                    {
                        var friction = _myTerrain.GetVehicleFriction(playerPos, vehicle.vehicleAssetInfo.Id);
                        gripFriction = friction.Grip;
                        dragFriction = friction.Drag;
                    }
                    else
                    {
                        gripFriction = _myTerrain.GetGripFriction(playerPos);
                        dragFriction = _myTerrain.GetDragFriction(playerPos);
                    }
                   
                }
                else
                {
                    gripFriction = _myTerrain.GetGripFriction(playerPos);
                    dragFriction = _myTerrain.GetDragFriction(playerPos);
                }
                
                textureId = _myTerrain.GetId(playerPos);
            }
        }
    }
}
