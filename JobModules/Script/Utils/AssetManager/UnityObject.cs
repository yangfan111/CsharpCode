using System;
using Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.AssetManager
{
    [DisallowMultipleComponent]
    public class UnityObjectReference : MonoBehaviour
    {
        public UnityObject Reference;
    }

    public class UnityObject : UnityObjectWrapper<Object>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityObject));

        private static int _globalSequence = 0;

        public Object AsObject
        {
            get { return Value; }
            set {Value = value;} 
        }

        public MonoBehaviour AudioMono { get; set; }

        public GameObject AsGameObject
        {
            get { return Value as GameObject; }
        }

        public void ToggleGameObject(bool val)
        {
            if(AsGameObject)
                AsGameObject.SetActive(val);
        }
        public IAssetPostProcessor _assetPostProcessor = null;

        public T As<T>() where T : Object
        {
            return Value as T;
        }

        public static implicit operator Object(UnityObject unityObj)
        {
            if (unityObj != null)
                return unityObj.AsObject;

            return null;
        }

        public static implicit operator GameObject(UnityObject unityObj)
        {
            if (unityObj != null)
                return unityObj.AsGameObject;

            return null;
        }

        public UnityObject(Object value, AssetInfo address) :
            base(value, address, _globalSequence++)
        {
        }

        public UnityObject WithPostProcessor(Func<UnityObject,IAssetPostProcessor> PostProcessorFactory)
        {
            if (PostProcessorFactory != null)
            {
                _assetPostProcessor = PostProcessorFactory(this);
            }

            return this;
        }

        public void SetActive(bool active)
        {
            var go = AsGameObject;
            if (go != null)
            {
                go.SetActive(active);

                if (!active)
                {
                    go.transform.SetParent(DefaultParent, false);
                }
            }
        }

        public void AddUnityObjectReference()
        {
            var go = AsGameObject;
            if (go != null)
            {
                var comp = go.GetComponent<UnityObjectReference>();
                if (comp == null)
                {
                    comp = go.AddComponentUncheckRequireAndDisallowMulti<UnityObjectReference>();
                }

                comp.Reference = this;
            }
        }

        public static UnityObject GetUnityObject(GameObject go)
        {
            if (go != null)
            {
                var comp = go.GetComponent<UnityObjectReference>();
                if (comp == null)
                {
                    throw new Exception(
                        "Can not get related unityobject, component 'UnityObjectReference' is not found");
                }

                return comp.Reference;
            }

            return null;
        }

        public static Transform DefaultParent
        {
            get { return DefaultGo.DefaultParent.transform; }
        }

        public void OnLoadFromAsset()
        {
            if (_assetPostProcessor != null)
            {
                _assetPostProcessor.LoadFromAsset(this);
            }
        }

        public void OnLoadFromPool()
        {
            if (_assetPostProcessor != null)
            {
                _assetPostProcessor.LoadFromPool(this);
            }
        }

        public void OnRecyle()
        {
            if (_assetPostProcessor != null)
            {
                _assetPostProcessor.Recyle(this);
            }
        }

        public void OnDestory()
        {
            if (_assetPostProcessor != null)
            {
                _assetPostProcessor.OnDestory(this);
            }
        }
    }
}