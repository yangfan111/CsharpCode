using System;
using App.Shared.SceneManagement;
using Core.Utils;
using UnityEngine;
using XmlConfig;
using Utils.AssetManager;
using Core.Geography;
using System.Collections.Generic;
using System.Collections;
using App.Shared.SceneTriggerObject;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.Configuration
{

    public interface IMapConfigManager
    {
        AbstractMapConfig SceneParameters { get; }
    }

    public class MapConfigManager : Singleton<MapConfigManager>, IMapConfigManager
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(MapConfigManager));

        public AbstractMapConfig SceneParameters { get; private set; }
        public ZoneController _zone;

        private HashSet<string> _loadedMapNames = new HashSet<string>();

        private static MapConfig _mapConfig;

        public string MapIconBundle;
        public string MapIconAsset;

        public MapConfigManager()
        { }


        public bool InWater(Vector3 position)
        {
            return _zone.InZone(SpecialZone.Water, position);
        }

        public float WaterSurfaceHeight(Vector3 position)
        {
            return _zone.DistanceInsideUpperBorder(SpecialZone.Water, position);
        }

        public float DistanceAboveWater(Vector3 position)
        {
            return _zone.DistanceOutsideUpperBorder(SpecialZone.Water, position);
        }

        public float GetHeightOfWater(Vector3 position)
        {
            return _zone.GetHeightOfUpperBorder(SpecialZone.Water, position);
        }

        public void SetMapInfo(AbstractMapConfig info)
        {
            SceneParameters = info;
            _zone = new ZoneController();
            _zone.AddZone(SceneParameters.SpecialZones);
        }
        
        public void AddLoadingMap(string mapName)
        {
            _loadedMapNames.Add(mapName);
        }

        public ArrayList GetLoadedMapNames() {
            ArrayList maps = new ArrayList();
            foreach (var mn in _loadedMapNames)
            {
                maps.Add(mn);
            }
            return maps;
        }

        public void LoadSpecialZoneTriggers()
        {
            _zone.CreateZoneTriggers(SpecialZone.Water, UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger));
        }
    }
}
