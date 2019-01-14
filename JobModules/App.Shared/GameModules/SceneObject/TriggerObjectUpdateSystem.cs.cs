using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using Core.GameModule.Interface;
using App.Shared.SceneTriggerObject;
using Utils.Singleton;

namespace App.Shared.GameModules.SceneObject
{
    public class TriggerObjectUpdateSystem : IGamePlaySystem
    {
        private TriggerObjectManager _manager;
        public TriggerObjectUpdateSystem()
        {
            _manager =  SingletonManager.Get<TriggerObjectManager>();
        }

        public void OnGamePlay()
        {
            _manager.ProcessLastLoadedObjects();
            _manager.ProcessLastUnloadedObjects();
        }
    }
}
