using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using App.Shared.SceneManagement;
using Core.GameModule.Interface;
using App.Shared.SceneTriggerObject;
using Core.SceneManagement;
using UnityEngine.SceneManagement;
using Utils.Singleton;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace App.Shared.GameModules.SceneObject
{
    public class TriggerObjectUpdateSystem : IGamePlaySystem
    {
        private TriggerObjectManager _manager;
        private Contexts _contexts;
        public TriggerObjectUpdateSystem(Contexts contexts)
        {
            _manager =  SingletonManager.Get<TriggerObjectManager>();
            _contexts = contexts;
        }
        
        
        public void OnGamePlay()
        {
            _manager.ProcessLastLoadedObjects();
            _manager.ProcessLastUnloadedObjects();
        }

    }
}
