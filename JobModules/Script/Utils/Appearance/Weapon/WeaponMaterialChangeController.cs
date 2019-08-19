using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.Weapon
{
    public class WeaponMaterialChangeController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponMaterialChangeController));

        private readonly LoadWeaponMaterialHandler[,] _loadWeaponMaterialHandlers =
            new LoadWeaponMaterialHandler[(int) WeaponInPackage.EndOfTheWorld, 2];

        private readonly List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();
        private readonly List<UnityObject> _recycleRequestBatch = new List<UnityObject>();

        private Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> _func;
        
        private UnityObject[] _materials = new UnityObject[(int) WeaponInPackage.EndOfTheWorld];
        
        private int _validP1Index = 0;
        private int _validP3Index = 1;

        public WeaponMaterialChangeController()
        {
            for (var i = 0; i < _loadWeaponMaterialHandlers.GetLength(0); ++i)
            {
                _loadWeaponMaterialHandlers[i, _validP1Index] = new LoadWeaponMaterialHandler(this, (WeaponInPackage) i);
                _loadWeaponMaterialHandlers[i, _validP3Index] = new LoadWeaponMaterialHandler(this, (WeaponInPackage) i);
            }
        }

        public void SetDelegation(Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> func)
        {
            _func = func;
        }

        public void ChangeWeaponMaterial(WeaponInPackage pos, WeaponGameObjectData weapon, int weaponAvatarId, int personIndex)
        {
            _loadWeaponMaterialHandlers[(int) pos, personIndex].SetInfo(weapon, weaponAvatarId);

            var weaponAvatarManager = SingletonManager.Get<WeaponAvatarConfigManager>();
            var assetInfo = weaponAvatarManager.GetTexWeapoonAsset(weaponAvatarId);
            if ((null == assetInfo.AssetName || assetInfo.AssetName.Equals(String.Empty)) &&
                (null == assetInfo.BundleName || assetInfo.BundleName.Equals(String.Empty)))
                Logger.ErrorFormat("ErrorWeaponAvatarId Try To ChangeWeaponMaterial  id:  {0}", weaponAvatarId);
            else if (null != _func)
                _loadRequestBatch.Add(_func.Invoke(
                    assetInfo,
                    _loadWeaponMaterialHandlers[(int) pos, personIndex]));
        }

        private void ChangeMaterial(GameObject weapon, UnityObject material, WeaponInPackage pos)
        {
            _materials[(int) pos] = material;
            if(null == weapon || null == material) return;

            foreach (var render in weapon.GetComponentsInChildren<MeshRenderer>())
            {
                render.material = material.AsObject as Material;
            }
        }

        public void RecycleMaterial(WeaponInPackage pos)
        {
            if(null != _materials[(int)pos]) 
                AddRecycleObject(_materials[(int)pos]);
        }

        public List<AbstractLoadRequest> GetLoadRequests()
        {
            return _loadRequestBatch;
        }

        private void AddRecycleObject(UnityObject obj)
        {
            if (obj != null)
            {
                _recycleRequestBatch.Add(obj);
            }
        }
        
        public List<UnityObject> GetRecycleRequests()
        {
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();
        }

        private class LoadWeaponMaterialHandler : ILoadedHandler
        {
            private readonly WeaponMaterialChangeController _dataSource;
            private readonly WeaponInPackage _pos;

            private int _id;
            private WeaponGameObjectData _obj;

            public LoadWeaponMaterialHandler(WeaponMaterialChangeController source, WeaponInPackage pos)
            {
                _dataSource = source;
                _pos = pos;
            }

            public void SetInfo(WeaponGameObjectData obj, int id)
            {
                _obj = obj;
                _id = id;
            }

            public void OnLoadSuccess<T>(T source, UnityObject unityObj)
            {
                if (null == unityObj.AsObject) return;

                Logger.InfoFormat("Load WeaponMaterial: {0}", unityObj.Address);

                if (_id < 0)
                {
                    _dataSource.AddRecycleObject(unityObj);
                    return;
                }

                var expectedAddr = SingletonManager.Get<WeaponAvatarConfigManager>().GetTexWeapoonAsset(_id);
                if (expectedAddr.Equals(unityObj.Address) && null != _obj)
                {
                    _dataSource.ChangeMaterial(_obj.PrimaryAsGameObject, unityObj, _pos);
                    _dataSource.ChangeMaterial(_obj.DeputyAsGameObject, unityObj, _pos);
                }
                else
                {
                    _dataSource.AddRecycleObject(unityObj);
                }
            }
        }
    }
}
