using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using App.Shared.SceneTriggerObject;
using Core.GameModule.Interface;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Preparation
{
    public class InitTriggerObjectManagerSystem : IModuleInitSystem
    {
        public  InitTriggerObjectManagerSystem(Contexts contexts)
        {
            var allMaps = SingletonManager.Get<MapsDescription>();

            switch (allMaps.CurrentLevelType)
            {
                case LevelType.SmallMap:
                    RegisterLoaderForSmallMap(contexts);
                    break;
                case LevelType.BigMap:
                    RegisterLoaderForBigMap(contexts);
                    break;
            }
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            
        }

        private void RegisterLoaderForSmallMap(Contexts contexts)
        {
            SceneObjManager sm = new SceneObjManager();
            contexts.session.commonSession.LevelManager.SceneLoaded += (scene, type) => sm.OnSceneLoaded(scene);
            contexts.session.commonSession.LevelManager.SceneUnloaded += scene => sm.OnSceneUnloaded(scene);
        }

        private void RegisterLoaderForBigMap(Contexts contexts)
        {
            var manager = SingletonManager.Get<TriggerObjectManager>();
            contexts.session.commonSession.LevelManager.AfterGoLoaded += gameObj => manager.OnMapObjLoaded(gameObj);
            contexts.session.commonSession.LevelManager.BeforeGoUnloaded += gameObj => manager.OnMapObjUnloaded(gameObj);
        }
    }
}
