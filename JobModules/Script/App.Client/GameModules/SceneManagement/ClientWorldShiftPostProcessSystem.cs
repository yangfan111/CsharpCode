using System.Collections.Generic;
using App.Client.SceneManagement;
using Core.Components;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.GameModules.SceneManagement
{
    public class ClientWorldShiftPostProcessSystem : IModuleInitSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WorldShiftManager));
        private readonly ICommonSessionObjects _session;

      
        public ClientWorldShiftPostProcessSystem(ICommonSessionObjects session)
        {
            _session = session;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _session.LevelManager.SceneLoaded +=  SingletonManager.Get<WorldShiftManager>().SceneLoaded;
            _session.LevelManager.SceneUnloaded +=  SingletonManager.Get<WorldShiftManager>().SceneUnloaded;
           
        }

    }
}