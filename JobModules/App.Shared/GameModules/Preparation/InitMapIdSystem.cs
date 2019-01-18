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
        
        public InitMapIdSystem(Contexts contexts)
        {
            _mapId = contexts.session.commonSession.RoomInfo.MapId;
        }

        public void OnInitModule(ILoadRequestManager manager)
        {
            SingletonManager.Get<MapsDescription>().SetMapId(_mapId);
            SingletonManager.Get<MapConfigManager>().SetMapInfo(SingletonManager.Get<MapsDescription>().SceneParameters);
            SingletonManager.Get<MapConfigManager>().LoadSpecialZoneTriggers();
            //if (SingletonManager.Get<MapsDescription>().CurrentLevelType == LevelType.BigMap)
            SingletonManager.Get<TerrainManager>().LoadTerrain(manager, SingletonManager.Get<MapConfigManager>().SceneParameters);
        }
    }
}