using App.Client.GPUInstancing.Core;
using App.Client.GPUInstancing.Core.Utils;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Client.SceneManagement.Vegetation
{
    public class TerrainRenderer : ITerrainRenderer
    {
        private bool _hasTerrain;

        private readonly GpuInstancingDetailOnTerrain _detailManager;
        
        public TerrainRenderer()
        {
            _detailManager = new GpuInstancingDetailOnTerrain(
                (ComputeShader) Resources.Load(Constants.CsPath.DetailInstantiationInResource),
                (ComputeShader) Resources.Load(Constants.CsPath.MergeBufferInResource),
                (ComputeShader) Resources.Load(Constants.CsPath.VisibilityDeterminationInResource));
        }
        
        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                var terrain = rootGameObject.GetComponentInChildren<Terrain>();
                if (terrain != null)
                {
                    var terrainData = terrain.terrainData;
                    _detailManager.Init(terrain, terrainData);

                    terrain.detailObjectDistance = 0;
                    _hasTerrain = true;

                    break;
                }
            }

        }

        public void SceneUnloaded(Scene scene)
        {
        }

        public void GoLoaded(UnityObject obj)
        {
        }

        public void GoUnloaded(UnityObject obj)
        {
        }

        public void SetCamera(Camera cam)
        {
            _detailManager.SetRenderingCamera(cam);
        }

        public void Draw()
        {
            if (_hasTerrain)
                _detailManager.Draw();
        }
    }
}