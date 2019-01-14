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
        private int _mapId;
        private Action _callBack;
        private ILoadRequestManager _loadSceneHandler;

        public AbstractMapConfig SceneParameters { get; private set; }
        public ZoneController _zone;

        private HashSet<string> _loadedMapNames = new HashSet<string>();

        private static MapConfig _mapConfig;

        public static void Generate(int mapId, Action callBack)
        {
            var instance = SingletonManager.Get<MapConfigManager>();
            instance.Init(mapId,  callBack);
        }

        public static MapConfig MapConfig
        {
            get { return _mapConfig; }
        }

        public MapConfigManager()
        { }

        private void Init(int mapId, Action callBack)
        {
            _mapId = mapId;
            _callBack = callBack;
        }

        public void SetLoadSceneHandler(ILoadRequestManager loadSceneHandler)
        {
            _loadSceneHandler = loadSceneHandler;
        }

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

        public void OnLoadSucc(object source, AssetInfo assetInfo, UnityEngine.Object obj)
        {
            var asset = obj as TextAsset;
            if (null == asset)
            {
                Logger.ErrorFormat("Asset {0}:{1} Load Fialed ", assetInfo.BundleName, assetInfo.AssetName);
                return;
            }
            if (string.IsNullOrEmpty(asset.text))
            {
                Logger.ErrorFormat("MapConfig is Empty");
                return;
            }
            
            MapConfig cfg = null;
            try
            {
                cfg = XmlConfigParser<MapConfig>.Load(asset.text);
            }
            catch (Exception e)
            {
                Logger.Error("Parse MapConfig Error", e);
                return;
            }
            Logger.InfoFormat("OnLoadSucc mapInfo count:{0}",cfg.MapInfos.Count);
            SceneParameters = null;
            _mapConfig = cfg;
            foreach (var v in cfg.MapInfos)
            {
                if (v.Id == _mapId)
                {
                    SceneParameters = v;

                    _zone = new ZoneController();
                    _zone.AddZone(SceneParameters.SpecialZones);

                    IOperationAfterConfigLoaded op = null;
                    if (SceneParameters is LevelConfig)
                    {
                        op = new LevelInit(SingletonManager.Get<TriggerObjectManager>());
                    }
                    else if (SceneParameters is SceneConfig)
                    {
                        op = new TerrainInit(SingletonManager.Get<TriggerObjectManager>());
                    }

                    if (op != null)
                    {
                        if (SharedConfig.IsServer)
                        {
                            op.ServerOperation(v, _callBack);
                        }
                        else
                        {
                            op.ClientOperation(v, _callBack);
                        }

                        foreach (var req in op.LoadInitialScene())
                        {
                            _loadedMapNames.Add(req.Address.AssetName);
                            _loadSceneHandler.AddSceneRequest(req);
                        }
                    }

                    break;
                }
            }

            if (SceneParameters == null)
            {
                Logger.ErrorFormat("mapId:{0} is invalid", _mapId);
            }
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
            _zone.CreateZoneTriggers(SpecialZone.Water, UnityLayers.WaterTriggerLayer);
        }
    }
}
