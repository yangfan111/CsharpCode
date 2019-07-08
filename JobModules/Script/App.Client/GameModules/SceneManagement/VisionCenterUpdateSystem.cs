using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Client.SceneManagement;
using App.Client.SceneManagement.DistanceCulling;
using App.Shared;
using App.Shared.SceneManagement;
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
        private readonly ITerrainRenderer _terrainRenderer;
        private List<AssetInfo> _sceneRequests = new List<AssetInfo>();
        private List<AssetInfo> _goRequests = new List<AssetInfo>();
        private List<IEnumerable<AssetInfoEx<MeshRenderer>>> _lightmapsRequests = new List<IEnumerable<AssetInfoEx<MeshRenderer>>>();

        public VisionCenterUpdateSystem(Contexts contexts)
        {
            _levelManager = contexts.session.commonSession.LevelManager;
            if (contexts.session.clientSessionObjects.TerrainRenderer != null)
            {
                _terrainRenderer = contexts.session.clientSessionObjects.TerrainRenderer;
                Camera.main.gameObject.transform.position = contexts.session.commonSession.InitPosition;
                _terrainRenderer.SetCamera(Camera.main);
            }
        }

        public void OnRender()
        {
            _levelManager.UpdateOrigin(Camera.main.transform.position.WorldPosition());
            if (_terrainRenderer != null)
                _terrainRenderer.Draw();
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            _levelManager.GetRequests(_sceneRequests, _goRequests, _lightmapsRequests);

            foreach (var request in _sceneRequests)
            {
                assetManager.LoadSceneAsync(request, true);
                _logger.InfoFormat("load scene request {0}", request.AssetName);
            }

            foreach (var request in _goRequests)
            {
                _levelManager.LoadResource("VisionCenterUpdateSystem", assetManager, request);
            }

            foreach (var request in _lightmapsRequests)
            {
                MeshRenderer mr = null;
                List<AssetInfo> infos = new List<AssetInfo>();
                foreach (var ex in request)
                {
                    infos.Add(ex.asset);
                    if (mr == null) mr = ex.data;
                }
                LevelManager lm = _levelManager as LevelManager;
                assetManager.LoadAssetsAsync(mr, infos, lm.LightmapsLoadedWrapper);
            }

            _sceneRequests.Clear();
            _goRequests.Clear();
            _lightmapsRequests.Clear();
        }
    }
}