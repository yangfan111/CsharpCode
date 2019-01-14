using System;
using System.Collections.Generic;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.Enums;
using Core.Utils;
using Shared.Scripts.Terrains;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;
using VehicleCommon;
using XmlConfig;

namespace App.Shared.Terrains
{
    enum EAssetType
    {
        BYTES,
        AUDIO,
        EFFECT,
        MATERIAL
    }

    public class MyTerrain : IMyTerrain
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(MyTerrain));

        public int _mapId { get; set; }
        public string _mapName { get; set; }
        public bool _isLoaded { get; set; }

        private int _mapWidth = 1000;
        private int _mapHeight = 1000;
        private int _splitWidth = 1000;
        private int _splitHeight = 1000;
        private float _splitWidthInv = 0.001f;
        private float _splitHeightInv = 0.001f;
        private int _splitRows = 1;
        private int _splitCols = 1;
        private TerrainItem[] _terrains;
        //<SoundId, AudioClip>
        private Dictionary<int, AudioClip> _dictSounds = new Dictionary<int, AudioClip>();
        //<MaterialId, Material>
        private Dictionary<int, Material> _dictMaterials = new Dictionary<int, Material>();
        //Effect Pool
        private TerrainEffectPool _effectPool = new TerrainEffectPool();
        //Loaded Assets
        private HashSet<string> setLoadedAssets = new HashSet<string>();

        private Vector3 _terrainInitPos = new Vector3();
        private Vector3 _terrainOriginPos = new Vector3();
        private Vector3 _size = new Vector3();
        private float _defaultFrictionGrip = 2.3f;
        private float _defaultFrictionDrag = 0.1f;
        private int _defaultId = 0;
        private int _defaultSoundId = 0;
        private int _defaultEffectId = 0;
        private int _defaultMaterialId = 0;

        private ILoadRequestManager _manager;
        private System.Random _randGen = new System.Random();

        private static AssetInfo GetAssetInfo(string mapName, int subId)
        {
            return new AssetInfo("terrains/" + mapName, "TerrainData_" + subId);
        }

        public MyTerrain(ILoadRequestManager manager, AbstractMapConfig sceneConfig)
        {
            _manager = manager;
            _isLoaded = false;
            if (sceneConfig != null)
            {
                _mapId = sceneConfig.Id;
                _mapName = sceneConfig.MapName;
                _terrainOriginPos.Set(sceneConfig.OriginPosition.x, sceneConfig.OriginPosition.y, sceneConfig.OriginPosition.z);
                _size.Set(sceneConfig.Size.x, sceneConfig.Size.y, sceneConfig.Size.z);
                if (sceneConfig is SceneConfig)
                {
                    SceneConfig config = sceneConfig as SceneConfig;
                    _splitWidth = config.TerrainSize;
                    _splitHeight = config.TerrainSize;
                    _splitWidthInv = 1.0f / _splitWidth;
                    _splitHeightInv = 1.0f / _splitHeight;
                    _mapWidth = config.TerrainSize * config.TerrainDimension;
                    _mapHeight = config.TerrainSize * config.TerrainDimension;
                    Vector3 pos = config.TerrainMin;
                    _terrainInitPos.Set(pos.x, pos.y, pos.z);
                }
                else
                {
                    LevelConfig config = sceneConfig as LevelConfig;
                    _splitWidth = config.TerrainSize;
                    _splitHeight = config.TerrainSize;
                    _splitWidthInv = 1.0f / _splitWidth;
                    _splitHeightInv = 1.0f / _splitHeight;
                    _mapWidth = config.TerrainSize * config.TerrainDimension;
                    _mapHeight = config.TerrainSize * config.TerrainDimension;
                    Vector3 pos = config.TerrainMin;
                    _terrainInitPos.Set(pos.x, pos.y, pos.z);
                }
                _splitCols = (int)Math.Ceiling(_mapWidth / (float)_splitWidth);
                _splitRows = (int)Math.Ceiling(_mapHeight / (float)_splitHeight);
            }
            _terrains = new TerrainItem[_splitRows * _splitCols];
        }

        private TerrainTextureConfigItem GetTexture(int textureId)
        {
            return SingletonManager.Get<TerrainTextureConfigManager>().GetTextureById(textureId);
        }

        private int CalcPos2SubId(int posX, int posY)
        {
            int colInx =(int) (posX * _splitWidthInv);
            int rowInx = (int) (posY * _splitHeightInv);
            return GetSubId(rowInx, colInx);
        }

        private int GetSubId(int rowInx, int colInx)
        {
            return rowInx * _splitCols + colInx;
        }

        private TerrainItem GetSubTerrain(int subId)
        {
            if (subId < _terrains.Length)
            {
                //if (_terrains[subId] == null)
                //    LoadOne(subId);
                return _terrains[subId];
            }
            return null;
        }

        private void LoadOne(int subId)
        {
            if (subId >= _terrains.Length)
                return;
            if (_terrains[subId] != null)
                return;

            AssetInfo assetInfo = GetAssetInfo(_mapName, subId);
            int[] para = {(int)EAssetType.BYTES, subId};
            _manager.AppendLoadRequest(para, assetInfo, OnLoadSucc);
        }

        private bool IsNeedLoad(EAssetType type, int id)
        {
            string tag = type.ToString() + "_" + id;
            if (setLoadedAssets.Contains(tag))
            {
                return false;
            }
            else
            {
                setLoadedAssets.Add(tag);
                return true;
            }
        }

        private void LoadSoundAsset(int soundId)
        {
            var config = SingletonManager.Get<SoundConfigManager>().GetSoundById(soundId);
            if (null != config && IsNeedLoad(EAssetType.AUDIO, soundId))
            {
                AssetInfo assetInfo = new AssetInfo(config.Asset.BundleName, config.Asset.AssetName);
                int[] para = { (int)EAssetType.AUDIO, soundId };
                _manager.AppendLoadRequest(para, assetInfo, OnGoLoadSucc);
            }
        }

        private void LoadEffectAsset(int effectId)
        {
            TerrainEffectConfigItem config = SingletonManager.Get<TerrainEffectConfigManager>().GetEffectById(effectId);
            if (null != config && IsNeedLoad(EAssetType.EFFECT, effectId))
            {
                AssetInfo assetInfo = new AssetInfo(config.Asset.BundleName, config.Asset.AssetName);
                int[] para = { (int)EAssetType.EFFECT, effectId };
                _manager.AppendLoadRequest(para, assetInfo, OnGoLoadSucc);
            }
        }

        private void LoadMaterialAsset(int materialId)
        {
            TerrainMaterialConfigItem config = SingletonManager.Get<TerrainMaterialConfigManager>().GetMaterialById(materialId);
            if (null != config && IsNeedLoad(EAssetType.MATERIAL, materialId))
            {
                AssetInfo assetInfo = new AssetInfo(config.Asset.BundleName, config.Asset.AssetName);
                int[] para = { (int)EAssetType.MATERIAL, materialId };
                _manager.AppendLoadRequest(para, assetInfo, OnLoadSucc);
            }
        }

        private void LoadAssets(TerrainItem item)
        {
            foreach (int textureId in item._mTextureIds)
            {
                TerrainTextureConfigItem textureCfg = SingletonManager.Get<TerrainTextureConfigManager>().GetTextureById(textureId);
                if (null == textureCfg)
                    continue;
                TerrainTextureTypeConfigItem typeCfg =
                    SingletonManager.Get<TerrainTextureTypeConfigManager>().GetTextureTypeById(textureCfg.Type);
                if (null == typeCfg)
                    continue;
                
                //Sound
                LoadSoundAsset(typeCfg.SoundInfo.Normal);
                LoadSoundAsset(typeCfg.SoundInfo.Brake);
                foreach (int id in typeCfg.SoundInfo.WalkIds)
                {
                    LoadSoundAsset(id);
                }
                //Effects
                LoadEffectAsset(typeCfg.EffectInfo.Normal);
                LoadEffectAsset(typeCfg.EffectInfo.Brake);
                LoadEffectAsset(typeCfg.EffectInfo.BrokenBrake);
                //Material
                LoadMaterialAsset(typeCfg.MaterialInfo.Track);
                LoadMaterialAsset(typeCfg.MaterialInfo.Slippery);
            }
        }

        public void LoadAll()
        {
            //TerrainData
            for (int rowInx = 0; rowInx < _splitRows; rowInx++)
            {
                for (int colInx = 0; colInx < _splitCols; colInx++)
                {
                    int subId = GetSubId(rowInx, colInx);
                    LoadOne(subId);
                }
            }
        }

        public Vector3 InitPosition
        {
            get
            {
                return _terrainInitPos;
            }
        }
        public Vector3 OriginPosition
        {
            get
            {
                return _terrainOriginPos;
            }
        }
        public Vector3 Size
        {
            get
            {
                return _size;
            }
        }

        public float TerrainSize
        {
            get { return Math.Min(_mapWidth, _mapHeight); }
        }

        private static TerrainTextureTypeConfigItem GetTextureType(int textureId)
        {
            TerrainTextureConfigItem item = SingletonManager.Get<TerrainTextureConfigManager>().GetTextureById(textureId);
            if (null != item)
            {
                return SingletonManager.Get<TerrainTextureTypeConfigManager>().GetTextureTypeById(item.Type);
            }
            return null;
        }

        public STFriction GetFriction(Vector3 worldPos)
        {
            /*
            int posX = Math.Max((int)(worldPos.x - _terrainInitPos.x), 0);
            int posZ = Math.Max((int)(worldPos.z - _terrainInitPos.z), 0);
            int subId = CalcPos2SubId(posX, posZ);
            TerrainItem item = GetSubTerrain(subId);
            if (item != null)
            {
                posX %= _splitWidth;
                posZ %= _splitHeight;
                return item.GetFriction(posX, posZ, _defaultFrictionGrip, _defaultFrictionDrag);
            }*/
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem item = GetTextureType(textureId);
            if (null != item)
            {
                return new STFriction(item.Grip, item.Drag);
            }
            return new STFriction(_defaultFrictionGrip, _defaultFrictionDrag);
        }

        //取摩擦力
        public float GetGripFriction(Vector3 worldPos)
        {
            /*
            int posX = Math.Max((int)(worldPos.x - _terrainInitPos.x), 0);
            int posZ = Math.Max((int)(worldPos.z - _terrainInitPos.z), 0);
            int subId = CalcPos2SubId(posX, posZ);
            TerrainItem item = GetSubTerrain(subId);
            if (item != null)
            {
                posX %= _splitWidth;
                posZ %= _splitHeight;
                return item.GetGripFriction(posX, posZ, _defaultFrictionGrip);
            }*/
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem item = GetTextureType(textureId);
            if (null != item)
            {
                return item.Grip;
            }
            return _defaultFrictionGrip;
        }

        //取摩擦力
        public float GetDragFriction(Vector3 worldPos)
        {
            /*
            int posX = Math.Max((int)(worldPos.x - _terrainInitPos.x), 0);
            int posZ = Math.Max((int)(worldPos.z - _terrainInitPos.z), 0);
            int subId = CalcPos2SubId(posX, posZ);
            TerrainItem item = GetSubTerrain(subId);
            if (item != null)
            {
                posX %= _splitWidth;
                posZ %= _splitHeight;
                return item.GetDragFriction(posX, posZ, _defaultFrictionDrag);
            }*/
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem item = GetTextureType(textureId);
            if (null != item)
            {
                return item.Drag;
            }
            return _defaultFrictionDrag;
        }

        //Vehicle Friction
        public STFriction GetVehicleFriction(Vector3 worldPos, int vehicleId)
        {
            int textureId = GetId(worldPos);
            TerrainTextureConfigItem texture = SingletonManager.Get<TerrainTextureConfigManager>().GetTextureById(textureId);
            if (null != texture)
            {
                TerrainVehicleFrictionConfigItem item = SingletonManager.Get<TerrainVehicleFrictionConfigManager>().GetFrictionById(vehicleId, texture.Type);
                if (null != item)
                {
                    return new STFriction(item.Grip, item.Drag);
                }
            }
            return new STFriction(_defaultFrictionGrip, _defaultFrictionDrag);
        }

        //取TextureId
        public int GetId(Vector3 worldPos)
        {
            int posX = Math.Max((int)(worldPos.x - _terrainInitPos.x), 0);
            int posZ = Math.Max((int)(worldPos.z - _terrainInitPos.z), 0);
            int subId = CalcPos2SubId(posX, posZ);
            TerrainItem item = GetSubTerrain(subId);
            if (item != null)
            {
                posX %= _splitWidth;
                posZ %= _splitHeight;
                return item.GetTextureId(posX, posZ, _defaultId);
            }
            return _defaultId;
        }

        public int GetSoundId(Vector3 worldPos, ETerrainSoundType soundType)
        {
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem texture = GetTextureType(textureId);
            if (null != texture)
            {
                switch (soundType)
                {
                    case ETerrainSoundType.Normal:
                        return texture.SoundInfo.Normal;
                    case ETerrainSoundType.Brake:
                        return texture.SoundInfo.Brake;
                    case ETerrainSoundType.Walk:
                        int[] ids = texture.SoundInfo.WalkIds;
                        if (null != ids && ids.Length > 0)
                            return ids[_randGen.Next(ids.Length)];
                        break;
                    case ETerrainSoundType.Squat:
                        var squatIds = texture.SoundInfo.SquatIds;
                        if (null != squatIds && squatIds.Length > 0)
                            return squatIds[_randGen.Next(squatIds.Length)];
                        break;
                    case ETerrainSoundType.Crawl:
                        var crawIds = texture.SoundInfo.CrawlIds;
                        if (null != crawIds && crawIds.Length > 0)
                            return crawIds[_randGen.Next(crawIds.Length)];
                        break;
                    case ETerrainSoundType.Land:
                        return texture.SoundInfo.Land;
                }
            }
            return _defaultSoundId;
        }

        public AudioClip GetSound(Vector3 worldPos, ETerrainSoundType soundType)
        {
            int soundId = GetSoundId(worldPos, soundType);
            AudioClip audio = null;
            _dictSounds.TryGetValue(soundId, out audio);
            return audio;
        }

        //EffectId
        public int GetEffectId(Vector3 worldPos, ETerrainEffectType effectType)
        {
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem texture = GetTextureType(textureId);
            if (null != texture)
            {
                switch (effectType)
                {
                    case ETerrainEffectType.Normal:
                        return texture.EffectInfo.Normal;
                    case ETerrainEffectType.Brake:
                        return texture.EffectInfo.Brake;
                    case ETerrainEffectType.BrokenBrake:
                        return texture.EffectInfo.BrokenBrake;
                }
            }
            return _defaultEffectId;
        }

        //EffectItem
        public ITerrainEffectItem GetEffect(Vector3 worldPos, ETerrainEffectType effectType)
        {
            int effectId = GetEffectId(worldPos, effectType);
            return _effectPool.GetNewEffect(effectId);
        }

        //Release Effect
        public void ReleaseEffect(ITerrainEffectItem item)
        {
            _effectPool.ReleaseEffect(item);
        }

        //MaterialId
        public int GetMaterialId(Vector3 worldPos, ETerrainMaterialType materialType)
        {
            int textureId = GetId(worldPos);
            TerrainTextureTypeConfigItem texture = GetTextureType(textureId);
            if (null != texture)
            {
                switch (materialType)
                {
                    case ETerrainMaterialType.Track:
                        return texture.MaterialInfo.Track;
                    case ETerrainMaterialType.Slippery:
                        return texture.MaterialInfo.Slippery;
                }
            }
            return _defaultMaterialId;
        }

        //Material
        public Material GetMaterial(Vector3 worldPos, ETerrainMaterialType materialType)
        {
            int materialId = GetMaterialId(worldPos, materialType);
            Material material = null;
            _dictMaterials.TryGetValue(materialId, out material);
            return material;
        }

        public void OnLoadSucc(object source, AssetInfo assetInfo, UnityEngine.Object obj)
        {
            if (null == obj)
            {
                _logger.ErrorFormat("Asset {0}:{1} Load Fialed ", assetInfo.BundleName, assetInfo.AssetName);
                return;
            }
            int[] para = (int[]) source;
            if (para == null || para.Length != 2)
            {
                return;
            }
            try
            {
                EAssetType atype = (EAssetType)para[0];
                switch (atype)
                {
                    case EAssetType.BYTES:
                    {
                        //TerrainData
                        var asset = obj as TextAsset;
                        if (null == asset.bytes)
                        {
                            _logger.ErrorFormat("TerrainConfig is NULL");
                            return;
                        }
                        int subId = para[1];
                        TerrainImporter import = new TerrainImporter();
                        TerrainItem item = new TerrainItem();
                        import.ImportBytesConfig(asset.bytes, item);
                        _terrains[subId] = item;
                        LoadAssets(item);
                        break;
                    }
                    case EAssetType.MATERIAL:
                    {
                        //Material
                        var asset = obj as Material;
                        if (null == asset)
                        {
                            return;
                        }
                        int materialId = para[1];
                        _dictMaterials.Add(materialId, asset);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(e.Message);
            }
        }

        public void OnGoLoadSucc(object source, UnityObjectWrapper<GameObject> obj)
        {
            if (null == obj)
            {
                _logger.ErrorFormat("Load GameObject Asset Fialed: obj is null.");
                return;
            }
            if (null == obj.Value)
            {
                _logger.ErrorFormat("Asset {0}:{1} Load GameObject Fialed ", obj.Address.BundleName, obj.Address.AssetName);
                return;
            }

            int[] para = (int[])source;
            if (para == null || para.Length != 2)
            {
                return;
            }

            try
            {
                EAssetType atype = (EAssetType)para[0];
                switch (atype)
                {
                    case EAssetType.AUDIO:
                    {
                        //Sound
                        int soundId = para[1];
                        if (!_dictSounds.ContainsKey(soundId))
                        {
                            AudioSource audio = obj.Value.GetComponent<AudioSource>();
                            if (null != audio)
                                _dictSounds.Add(soundId, audio.clip);
                        }
                        break;
                    }
                    case EAssetType.EFFECT:
                    {
                        //Effect
                        int effectId = para[1];
                        _effectPool.AddEffectPrefab(effectId, obj.Value);
                        break;
                    }
                }
                _manager.AddRecycleRequest(obj);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(e.Message);
            }
        }

        public VehicleFriction GetFrictionAt(Vector3 position, int vehicleId)
        {
            var friction = new VehicleFriction();
            var terrainFriction = GetVehicleFriction(position, vehicleId);
            friction.Grip = terrainFriction.Grip;
            friction.Drag = terrainFriction.Drag;

            return friction;
        }

        public AudioClip GetAudioAt(Vector3 position, VehicleAudioIndex index)
        {
            switch (index)
            {
                case VehicleAudioIndex.Running:
                    return GetSound(position, ETerrainSoundType.Normal);
                case VehicleAudioIndex.Brake:
                    return GetSound(position, ETerrainSoundType.Brake);
                case VehicleAudioIndex.HandBrake:
                    return null;
                case VehicleAudioIndex.BrokenRunning:
                    return null;
                default:
                    throw new Exception(String.Format("Undefined Ground Audio for index {0}!", index));
            }
        }


        public void BorrowEffectAt(Vector3 position, VehicleEffectIndex index, VehicleEffectData groundEffect)
        {
            ETerrainEffectType effectType;
            switch (index)
            {
                case VehicleEffectIndex.WheelRunning:
                    effectType = ETerrainEffectType.Normal;
                    break;
                case VehicleEffectIndex.WheelBrake:
                    effectType = ETerrainEffectType.Brake;
                    break;
                default:
                    throw new Exception(String.Format("Undefined Ground Effect for index {0}!", index));
            }

            var effect = GetEffect(position, effectType);
            if (effect != null)
            {
                groundEffect.Id = GetEffectId(position, effectType);
                groundEffect.Effect = effect.EffectGo;
                groundEffect.Effect.SetActive(false);
                groundEffect.RawObject = effect;
            }
            else
            {
                groundEffect.Id = 0;
                groundEffect.Effect = null;
                groundEffect.RawObject = null;
            }
           
        }

        public void ReturnEffect(VehicleEffectData groundEffect)
        {
            if (groundEffect.RawObject != null)
            {
                ReleaseEffect((ITerrainEffectItem) groundEffect.RawObject);
                groundEffect.Effect = null;
                groundEffect.RawObject = null;
            }
        }

        public Material GetMaterial(Vector3 position, VehicleMaterialIndex index)
        {
            switch (index)
            {
                case VehicleMaterialIndex.TireMark:
                    return GetMaterial(position, ETerrainMaterialType.Track);
                case VehicleMaterialIndex.SkidMark:
                    return GetMaterial(position, ETerrainMaterialType.Slippery);
                default:
                    throw new Exception(String.Format("Undefined Ground Material for index {0}!", index));
            }
        }
    }
}
