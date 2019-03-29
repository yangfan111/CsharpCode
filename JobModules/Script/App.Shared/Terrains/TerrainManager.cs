using System.Collections.Generic;
using App.Shared.Configuration;
using Utils.AssetManager;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Terrains
{
    public interface ITerrainManager
    {
        IMyTerrain GetTerrain(int mapId);
    }

    public class TerrainManager : Singleton<TerrainManager>, ITerrainManager 
    {
        private Dictionary<int, IMyTerrain> _dictTerrains = new Dictionary<int, IMyTerrain>();

        public TerrainManager()
        {
            
        }

        public void LoadTerrain(IUnityAssetManager assetManager, AbstractMapConfig sceneConfig)
        {
            if (!_dictTerrains.ContainsKey(sceneConfig.Id))
            {
                MyTerrain terrain = new MyTerrain(assetManager, sceneConfig);
                terrain.LoadAll();
                AddTerrain(terrain);
            }
            SetTerrainData();
        }

        private void AddTerrain(IMyTerrain terrain)
        {
            if (!_dictTerrains.ContainsKey(terrain._mapId))
            {
                _dictTerrains.Add(terrain._mapId, terrain);
            }
        }

        public IMyTerrain GetTerrain(int mapId)
        {
            IMyTerrain terrain;
            _dictTerrains.TryGetValue(mapId, out terrain);
            return terrain;
        }

        public IMyTerrain GetCurrentTerrain()
        {
            IMyTerrain terrain;
            _dictTerrains.TryGetValue(SingletonManager.Get<MapConfigManager>().SceneParameters.Id, out terrain);
            return terrain;
        }

        public void RemoveTerrain(int mapId)
        {
            if (_dictTerrains.ContainsKey(mapId))
            {
                _dictTerrains.Remove(mapId);
            }
        }

        private void SetTerrainData()
        {
            IMyTerrain myTerrain = GetCurrentTerrain();
            if (null != myTerrain)
            {
//                TerrainCommonData.leftMinPos = new Vector3(myTerrain.OriginPosition.x, myTerrain.OriginPosition.y,myTerrain.OriginPosition.z);
//                TerrainCommonData.size = new Vector2(myTerrain.Size.x, myTerrain.Size.y);
            }
        }
    }
}
