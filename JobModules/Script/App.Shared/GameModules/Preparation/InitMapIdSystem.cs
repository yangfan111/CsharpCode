using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.GameModule.Interface;
using Entitas;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Preparation
{
    public class InitMapIdSystem : IModuleInitSystem
    {
        private int _mapId;
        private Contexts _contexts;

        public InitMapIdSystem(Contexts contexts)
        {
            _contexts = contexts;
            _mapId = contexts.session.commonSession.RoomInfo.MapId;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            SingletonManager.Get<MapsDescription>().SetMapId(_mapId);
            if (SharedConfig.IsOffline)
            {
                _contexts.session.commonSession.InitPosition =
                    SingletonManager.Get<MapsDescription>().SceneParameters.PlayerBirthPosition;
            }

            SingletonManager.Get<MapConfigManager>().SetMapInfo(SingletonManager.Get<MapsDescription>().SceneParameters);
            SingletonManager.Get<MapConfigManager>().LoadSpecialZoneTriggers();
            //if (SingletonManager.Get<MapsDescription>().CurrentLevelType == LevelType.BigMap)
            SingletonManager.Get<TerrainManager>().LoadTerrain(assetManager, SingletonManager.Get<MapConfigManager>().SceneParameters);

            TerrainCommonData.size = SingletonManager.Get<MapConfigManager>().SceneParameters.Size;
            TerrainCommonData.leftMinPos = SingletonManager.Get<MapConfigManager>().SceneParameters.OriginPosition;


        }
    }
}