//#define UnitTest
using Assets.Utils.Configuration;
using Utils.AssetManager;
using Core.Utils;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;
using Utils.Singleton;

namespace Utils.Appearance.Weapon
{
    public class WeaponModelController : IWeaponModelController
    {
#if !UnitTest
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponModelController));
#endif
        //private IWeaponResourceConfigManager  SingletonManager.Get<WeaponResourceConfigManager>();
        //private IWeaponPartsConfigManager  SingletonManager.Get<WeaponPartsConfigManager>();
        //private IWeaponAvatarConfigManager _weaponAvatarConfigManager;
        private IWeaponModelLoadController<object> _weaponModelLoadController;
        private class AssetItem
        {
            /// <summary>
            /// 配置ID
            /// </summary>
            public int Id;
            /// <summary>
            /// 资源路径
            /// </summary>
            public AssetInfo Asset;

            public void Set(int id, AssetInfo asset)
            {
                Id = id;
                Asset = asset;
            }

            public void CopyTo(AssetItem target)
            {
                target.Id = Id;
                target.Asset = Asset;
            }

            public void Clear()
            {
                Id = UniversalConsts.InvalidIntId;
                Asset.BundleName = string.Empty;
                Asset.AssetName = string.Empty;
            }
        }
        private AssetItem _weapon = new AssetItem();
        private AssetItem _lastWeapon = new AssetItem();
        /// <summary>
        /// 存储当前武器的默认配件信息
        /// </summary>
        Dictionary<EWeaponPartType, int> _deafultWeaponPartDic = new Dictionary<EWeaponPartType, int>(CommonIntEnumEqualityComparer<EWeaponPartType>.Instance);
        /// <summary>
        /// 以类型为索引存储当前配件信息
        /// </summary>
        Dictionary<EWeaponPartType, AssetItem> _weaponPartDic = new Dictionary<EWeaponPartType, AssetItem>(CommonIntEnumEqualityComparer<EWeaponPartType>.Instance);
        Dictionary<EWeaponPartType, AssetItem> _lastWeaponPartDic = new Dictionary<EWeaponPartType, AssetItem>(CommonIntEnumEqualityComparer<EWeaponPartType>.Instance);

        public WeaponModelController(
            //IWeaponResourceConfigManager weaponConfigManager,
            //IWeaponPartsConfigManager weaponPartsConfigManager,
            //IWeaponAvatarConfigManager weaponAvatarConfigManager,
            IWeaponModelLoadController<object> weaponModelLoadController)
        {
            // SingletonManager.Get<WeaponResourceConfigManager>() = weaponConfigManager;
            // SingletonManager.Get<WeaponPartsConfigManager>() = weaponPartsConfigManager;
            //_weaponAvatarConfigManager = weaponAvatarConfigManager;
            _weaponModelLoadController = weaponModelLoadController;
            InitPartDic();
        }

        private void InitPartDic()
        {
            for(var t = EWeaponPartType.None + 1; t < EWeaponPartType.Length; t++)
            {
                _deafultWeaponPartDic[t] = 0;
                _weaponPartDic[t] = new AssetItem
                {
                    Id = UniversalConsts.InvalidIntId,
                };
                _lastWeaponPartDic[t] = new AssetItem
                {
                    Id = UniversalConsts.InvalidIntId,
                };
            }
        }

        public void Clear()
        {
            _weaponModelLoadController.Clear();
        }

        public void SetPart(int partId)
        {
//            if(null ==  SingletonManager.Get<WeaponPartsConfigManager>())
//            {
//#if !UnitTest
//                Logger.Error("weapon parts config manager is null !");
//#endif
//                return;
//            }
            if(partId < 1)
            {
#if !UnitTest
                Logger.WarnFormat("illegal weapon parts id {0}", partId);
#endif
                return;
            }
            var config = SingletonManager.Get<WeaponPartsConfigManager>() .GetConfigById(partId);
            if(null == config)
            {
                return;
            }
            var partType = (EWeaponPartType)config.Type;
            if (!_weaponPartDic.ContainsKey(partType))
            {
#if !UnitTest
                Logger.ErrorFormat("weapon part type {0} doesn't exist in weaponpartdic ", partType);
#endif
                return;
            }

            var asset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(partId);
            var location = WeaponTypeConvertUtil.GetLocationByPartType((EWeaponPartType)config.Type);
            var item = _weaponPartDic[partType];
            item.Id = partId;
            item.Asset = asset;
            RefreshAssets();
        }

        /// <summary>
        /// 设置武器会清掉之前的武器及配件信息，即使是同一Id
        /// </summary>
        /// <param name="weaponId"></param>
        public void SetWeapon(int weaponId)
        {
//            if(null ==  SingletonManager.Get<WeaponResourceConfigManager>())
//            {
//#if !UnitTest
//                Logger.Error("weapon config manager is null !");
//#endif
//                return;
//            }
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("SetWeapon {0}", weaponId);
            }
#endif
            int avId = SingletonManager.Get<WeaponResourceConfigManager>().GetAvatarByWeaponId(weaponId);
            var avarMgr = SingletonManager.Get<WeaponAvatarConfigManager>();
            
            var asset = avarMgr.GetThirdPersonWeaponModel(avId);
            _weapon.Id = weaponId;
            _weapon.Asset = asset;
            ClearLastPartsAndSetDefaultOnes(weaponId);
        }

        private void ClearLastPartsAndSetDefaultOnes(int weaponId)
        {
            var parts = SingletonManager.Get<WeaponResourceConfigManager>().GetDefaultWeaponAttachments(weaponId);
            foreach(var partPair in _weaponPartDic)
            {
                partPair.Value.Clear();
            }
            foreach(var part in parts)
            {
                var partConfig =  SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(part);
                if(null == partConfig)
                {
                    continue;
                }
                var partType = (EWeaponPartType)partConfig.Type;
                _deafultWeaponPartDic[partType] = part;
                var partItem = _weaponPartDic[partType];
                partItem.Id = part;
                partItem.Asset = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(part);
            }
            RefreshAssets();
        }

        public void RemovePart(int partId)
        {
            var partCfg = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
            if(null == partCfg)
            {
                return;
            }
            var partType = (EWeaponPartType)partCfg.Type;
            if(!_weaponPartDic.ContainsKey(partType))
            {
#if !UnitTest
                Logger.ErrorFormat("type {0} doesn't exist in part dic", partType);
#endif
                return;
            }
            var partItem = _weaponPartDic[partType]; 
            partItem.Clear();
            if(_deafultWeaponPartDic.ContainsKey(partType))
            {
                var defaultPartId = _deafultWeaponPartDic[partType];
                _weaponPartDic[partType].Id = defaultPartId;
                var asset =  SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(defaultPartId);
                _weaponPartDic[partType].Asset = asset;
            }
            RefreshAssets();
        }

        private bool HasLastWeapon()
        {
            return _lastWeapon.Id > 0;
        }

        public void UnloadWeapon()
        {
            _weaponModelLoadController.Unload(_weapon.Asset);
            _lastWeapon.Id = 0;
            _weapon.Id = 0;
            _weapon.Asset = default(AssetInfo);
            for (var t = EWeaponPartType.None + 1; t < EWeaponPartType.Length; t++)
            {
                _deafultWeaponPartDic[t] = 0;
                if(!_lastWeaponPartDic[t].Id.Equals(UniversalConsts.InvalidIntId)) _weaponModelLoadController.Unload(_lastWeaponPartDic[t].Asset);
                _lastWeaponPartDic[t] = new AssetItem
                {
                    Id = UniversalConsts.InvalidIntId,
                };
            }
        }

        private void RefreshAssets()
        {
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("RefreshAssets cur {0} last {1}", _weapon.Asset, _lastWeapon.Asset);
            }
#endif
            if(_weapon.Id != _lastWeapon.Id)
            {
                if(HasLastWeapon())
                {
                    _weaponModelLoadController.Unload(_lastWeapon.Asset);
                }
                _weaponModelLoadController.LoadWeapon(_weapon.Asset);
            }

            foreach(var partPair in _weaponPartDic)
            {
                var lastPart = _lastWeaponPartDic[partPair.Key];
                if(partPair.Value.Id != lastPart.Id)
                {
                    _weaponModelLoadController.Unload(_lastWeaponPartDic[partPair.Key].Asset);
                    if(partPair.Value.Id > 0)
                    {
                        var location = WeaponTypeConvertUtil.GetLocationByPartType(partPair.Key);
                        _weaponModelLoadController.LoadPart(_weaponPartDic[partPair.Key].Asset, location);
                    }
                }
            }

            _weapon.CopyTo(_lastWeapon);
            foreach(var partPair in _weaponPartDic)
            {
                partPair.Value.CopyTo(_lastWeaponPartDic[partPair.Key]);
            }
        }
    }
}