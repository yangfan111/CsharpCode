using System;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.Appearance.Effects;
using Utils.AssetManager;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class WeaponLoadAssetHandler : ParamBase, ILoadedHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponLoadAssetHandler));
        
        private readonly Action<UnityObject, int> _action;
        private GameObject _rootGo;
        private AssetInfo _assetInfo = AssetInfo.EmptyInstance;
        private int _assetId;

        public WeaponLoadAssetHandler(Action<UnityObject, int> action)
        {
            _action = action;
        }

        public void SetInfo(AssetInfo assetInfo, int assetId)
        {
            _assetInfo = assetInfo;
            _assetId = assetId;
        }

        public AssetInfo GetAssetInfo()
        {
            return _assetInfo;
        }

        private static void SetObjParam(GameObject obj, int layer)
        {
            if (null == obj) return;
            BoneTool.CacheTransform(obj);
            var childCount = obj.transform.childCount;
            for (var i = 0; i < childCount; ++i)
            {
                obj.transform.GetChild(i).gameObject.layer = layer;
            }

            AppearanceUtils.EnableShadow(obj);
            ReplaceMaterialShaderBase.ResetShader(obj);
        }

        public void OnLoadSuccess<T>(T source, UnityObject unityObj)
        {
            if (null == unityObj || null == unityObj.AsGameObject)
            {
                _assetInfo = AssetInfo.EmptyInstance;
                return;
            }
            
            if (!unityObj.AsGameObject.activeSelf)
            {
                Logger.ErrorFormat("unityObj:  {0}  is unActive", unityObj.Address);
                _assetInfo = AssetInfo.EmptyInstance;
                return;
            }

            Logger.InfoFormat("Load Weapon: {0}", unityObj.Address);

            SetObjParam(unityObj, UnityLayerManager.GetLayerIndex(EUnityLayerName.Player));

            if(!unityObj.Address.Equals(_assetInfo))
            {
                AddRecycleObject(unityObj);
                _assetInfo = AssetInfo.EmptyInstance;
                return;
            }
            
            _action(unityObj, _assetId);
            
            EffectUtility.ReflushGodModeEffect(_rootGo, unityObj.AsGameObject);
            _assetInfo = AssetInfo.EmptyInstance;
        }
    }
}
