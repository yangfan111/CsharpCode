using App.Shared.Configuration;
using Core.GameModule.Interface;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Preparation
{
    public class GameObjectRecordSystem:IModuleInitSystem
    {
        public GameObjectRecordSystem(Contexts contexts)
        {
            var allMaps = SingletonManager.Get<MapsDescription>();
            if (allMaps.CurrentLevelType == LevelType.BigMap)
            {
                var scc = SingletonManager.Get<StaticColliderCounter>();
                contexts.session.commonSession.LevelManager.GoLoaded += gameObj => scc.RecordObj(gameObj);
            }
        }
        
        public void OnInitModule(IUnityAssetManager assetManager)
        {
        }
    }
}