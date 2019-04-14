using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Client.SceneManagement;
using App.Client.SceneManagement.DistanceCulling;
using App.Shared;
using Core.Components;
using Core.GameModule.Interface;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.GameModules.SceneManagement
{
    public class VisionCenterUpdateSystem : IRenderSystem, IResourceLoadSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VisionCenterUpdateSystem));

        private readonly ILevelManager _levelManager;
        private Contexts _contexts;
        private List<AssetInfo> _sceneRequests = new List<AssetInfo>();
        private List<AssetInfo> _goRequests = new List<AssetInfo>();

        public VisionCenterUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
            _levelManager = contexts.session.commonSession.LevelManager;
            Camera.main.transform.position = contexts.player.flagSelfEntity.position.Value;
        }

        public void OnRender()
        {
            var status = _levelManager.UpdateOrigin(Camera.main.transform.position.WorldPosition());
            if (SharedConfig.EnableDC)
            {
                if (SharedConfig.EnableOC && Camera.main.useOcclusionCulling != status.CloseToBuilding)
                    Camera.main.useOcclusionCulling = status.CloseToBuilding;
            }
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            _levelManager.GetRequests(_sceneRequests, _goRequests);
            
            foreach (var request in _sceneRequests)
            {
                assetManager.LoadSceneAsync(request, true);
                _logger.InfoFormat("load scene request {0}", request.AssetName);
            }

            foreach (var request in _goRequests)
            {
                assetManager.LoadAssetAsync("VisionCenterUpdateSystem", request, _levelManager.GoLoadedWrapper);
            }
            
            _sceneRequests.Clear();
            _goRequests.Clear();
        }
    }
}