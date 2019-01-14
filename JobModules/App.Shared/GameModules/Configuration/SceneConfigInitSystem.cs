using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.GameModule.System;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public class SceneConfigInitSystem : IModuleInitSystem
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(SceneConfigInitSystem));
        private ISessionState _sessionState;
        private int _finishedTimes;
        private ILoadRequestManager _manager;

        private const int ClientFinishedTime = 1;
        private const int ServerFinishedTime = 1;

        public SceneConfigInitSystem(ISessionState sessionState, ICommonSessionObjects sessionObjects)
        {
            _sessionState = sessionState;
            _sessionState.CreateExitCondition(typeof(SceneConfigInitSystem));
            MapConfigManager.Generate(sessionObjects.RoomInfo.MapId, ReduceFinishTimes);
            _logger.InfoFormat("SceneConfigInitSystem :mapId:{0}", sessionObjects.RoomInfo.MapId);
        }

        public void OnInitModule(ILoadRequestManager manager)
        {
            _manager = manager;
            SingletonManager.Get<MapConfigManager>().SetLoadSceneHandler(manager);
            if (SharedConfig.IsServer)
            {
                _finishedTimes = ServerFinishedTime;
            }
            else
            {
                _finishedTimes = ClientFinishedTime;
            }
            _logger.InfoFormat("OnInitModule :mapConfig");
            manager.AppendLoadRequest(null, new AssetInfo("tables", "mapConfig"), SingletonManager.Get<MapConfigManager>().OnLoadSucc);
        }


        private void ReduceFinishTimes()
        {
            if (--_finishedTimes == 0)
            {
                LoadTerrainConfig();
                LoadSpecialZoneTriggers();
                _sessionState.FullfillExitCondition(typeof(SceneConfigInitSystem));
            }
        }

        private void LoadTerrainConfig()
        {
            SingletonManager.Get<TerrainManager>().LoadTerrain(_manager, SingletonManager.Get<MapConfigManager>().SceneParameters);
        }

        private void LoadSpecialZoneTriggers()
        {
            SingletonManager.Get<MapConfigManager>().LoadSpecialZoneTriggers();
        }
    }
}
