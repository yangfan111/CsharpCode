using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Utils;
using Core.Utils;
using UnityEngine;

namespace Utils.AssetManager
{
    public interface IUnityObjectPool 
    {
        void Add(UnityObject unityObj,bool active = false);
        UnityObject GetOrNull(AssetInfo key, bool autoActive = true);
        void Clear();
    }

    public class DefaultUnityObjectPool : IUnityObjectPool
    {
        public void Add(UnityObject unityObj, bool active = false)
        {
            unityObj.OnDestory();
            unityObj.Destroy();
        }

        public UnityObject GetOrNull(AssetInfo key, bool autoActive = true)
        {
            return null;
        }

        public void Clear()
        {
            
        }
    }

    public class UnityObjectPool : IUnityObjectPool
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityObjectPool));

        private bool _onlyCacheGameObject;
        public bool OnlyCacheGameObject { set { _onlyCacheGameObject = value; }}

        public UnityObjectPool(bool onlyCacheGameObject = false)
        {
            _onlyCacheGameObject = onlyCacheGameObject;
        }

        private Dictionary<AssetInfo, ObjectPool<UnityObject>> _poolDict = new Dictionary<AssetInfo, ObjectPool<UnityObject>>(AssetInfo.AssetInfoIngoreCaseComparer.Instance);
        public void Add(UnityObject unityObj,bool active)
        {
            try
            {
                if(!active)
                {
                    unityObj.SetActive(false);
                }
                if (_onlyCacheGameObject)
                {
                    if (unityObj.AsGameObject != null)
                        Add(unityObj.Address, unityObj);
                    else
                    {
                        unityObj.OnDestory();
                        unityObj.Destroy();
                    }

                }else if(unityObj.AsObject != null)
                    Add(unityObj.Address, unityObj);

            }
            catch (Exception e)
            {
                _logger.ErrorFormat("UnityGameObjectPool key:{0}, Exception:{1}", unityObj.Address, e);
                //throw e;
            }
        }

        private void Add(AssetInfo key, UnityObject unityObj)
        {
            var pool = _poolDict.GetOrDefault(key);
            if (pool == null)
            {
                pool = new ObjectPool<UnityObject>();
                _poolDict.Add(key, pool);
            }
            pool.Add(unityObj);
        }

        public UnityObject GetOrNull(AssetInfo key, bool autoActive = true)
        {
            var pool = _poolDict.GetOrDefault(key);

            UnityObject unityObj = null;
            if (pool != null)
            {
                unityObj = pool.GetOrNull();
            }

            if (unityObj != null)
            {
                if(autoActive)
                    unityObj.SetActive(true);
                if (unityObj.AsObject == null)
                    unityObj = null;
            }

            return unityObj;
        }

        public void Clear()
        {
            foreach (var pool in _poolDict.Values)
            {
                foreach (var unityObj in pool)
                {
                    unityObj.OnDestory();
                    unityObj.Destroy();
                }
                pool.Clear();
            }

            _poolDict.Clear();
        }
    }
}
