using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using App.Shared.SceneTriggerObject;
using ArtPlugins;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Preparation
{
    public class InitTriggerObjectManagerSystem : IModuleInitSystem
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(InitTriggerObjectManagerSystem));
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
            contexts.session.commonSession.LevelManager.SceneLoaded += (scene, type) => sm.OnSceneLoaded_SmallMap(scene);
            contexts.session.commonSession.LevelManager.SceneUnloaded += scene => sm.OnSceneUnloaded(scene);
        }

        private void RegisterLoaderForBigMap(Contexts contexts)
        {
            SceneObjManager sm = new SceneObjManager();
            var manager = SingletonManager.Get<TriggerObjectManager>();
            if (SharedConfig.IsServer)
            {
//                contexts.session.commonSession.LevelManager.SceneLoaded +=
//                    (scene, type) => sm.OnSceneLoaded_BigMap(scene);
//                contexts.session.commonSession.LevelManager.SceneUnloaded += (scene) => sm.OnSceneUnloaded(scene);
                contexts.session.commonSession.LevelManager.AfterGoLoaded += gameObj => manager.OnMapObjLoaded(gameObj,gameObj.SceneObjAttr.Id);
                contexts.session.commonSession.LevelManager.BeforeGoUnloaded += gameObj => manager.OnMapObjUnloaded(gameObj);
            }
            else
            {
                contexts.session.commonSession.LevelManager.AfterGoLoaded += gameObj =>
                {
                    var id = gameObj.SceneObjAttr.Id;
                    manager.OnMapObjLoaded(gameObj, id);
                };
                contexts.session.commonSession.LevelManager.BeforeGoUnloaded += gameObj => manager.OnMapObjUnloaded(gameObj);
            }
        }
    }
}
