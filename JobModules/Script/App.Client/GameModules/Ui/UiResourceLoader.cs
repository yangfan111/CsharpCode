using Assets.UiFramework.Libs;
using Core.Utils;
using System;
using System.Collections.Generic;
using Sharpen;
using UnityEngine;
using Utils.AssetManager;
using Object = System.Object;

namespace App.Client.GameModules.Ui
{
    public class UiResourceLoader : IUiResourcesLoader
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(UiResourceLoader));

        private IUnityAssetManager _assetManager;
        private GameObject _uiLoaderRoot;

        public UiResourceLoader(IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
            if (_uiLoaderRoot == null) _uiLoaderRoot = GameObject.Find("uiLoaderRoot");
            if (_uiLoaderRoot == null)
            {
                _uiLoaderRoot = new GameObject("uiLoaderRoot");
                _uiLoaderRoot.SetActive(false);
                _uiLoaderRoot.AddComponent<Canvas>();
            }
        }

        public void RetriveSpriteAsync(string bundle, string name, Action<Sprite> callback)
        {
            RetriveSpriteAsync(new AssetInfo(bundle, name), callback);
        }

        public void RetriveSpriteAsync(AssetInfo assetInfo, Action<Sprite> callback)
        {
            if (string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName))
            {
                Logger.ErrorFormat("illegal params bundle {0}, name {0}", assetInfo.BundleName, assetInfo.AssetName);
                if (null != callback)
                {
                    callback(null);
                }
                return;
            }

            LoadAsync(assetInfo, (obj) =>
            {
                if (null != callback)
                {
                    callback(CreateSprite(obj));
                }
            });
        }

        public void UnLoad(string bundleName, string assetName, object source)
        {
            _assetManager.LoadCancel(source);
        }

        public void LoadToCache(AssetInfo info, int count)
        {
            for (int i = 0; i < count; i++)
            {
                LoadAsync(info, null, null);
            }
           
        }

        public void LoadAsync(string bundle, string name, Action<UnityEngine.Object> callback, GameObject parent = null)
        {
            LoadAsync(new AssetInfo(bundle, name), callback, parent);
        }

        public void LoadAsync(AssetInfo assetInfo, Action<UnityEngine.Object> callback, GameObject parent = null)
        {
            if (string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName))
            {
                Debug.LogError("asset error." + assetInfo.BundleName + "," + assetInfo.AssetName);
                return;
            }

            _assetManager.LoadAssetAsync("UiResourceLoader", assetInfo, (source, unityObj)=>{
                try
                {
                    if (callback  != null)
                    {
                        callback(unityObj);
                    }
                    else
                    {
                        AddToGameObjectPool(unityObj);
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("ui resource loaded failed: {0} {1}", e, assetInfo);
                }
            }, new AssetLoadOption(parent:_uiLoaderRoot,  recyclable:true));
        }

        public void AddToGameObjectPool(Object obj)
        {
            var unityObj = obj as UnityObject;
            if (unityObj == null)
            {
                unityObj = UnityObject.GetUnityObject(obj as GameObject);
            }

            _assetManager.Recycle(unityObj);
        }


        public void GetConfigAsync(string bundleName, string assetName, Action<TextAsset> callback)
        {
            if (string.IsNullOrEmpty(assetName) || string.IsNullOrEmpty(assetName))
            {
                Logger.ErrorFormat("config load info is illegal with bundle{0}, name {0}", bundleName, assetName);
                return;
            }

            //TODO 这里现在有内存分配，需要优化
            LoadAsync(bundleName, assetName, (obj) =>
            {
                if (null != callback)
                {
                    callback(obj as TextAsset);
                }
            });
        }

        private Sprite CreateSprite(object src)
        {

            var tex = src as Texture2D;
            if (null != tex)
            {
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }

            var sprite = src as Sprite;
            if (null != sprite)
            {
                return sprite;
            }

            return null;
        }

        public void LoadUIEffectAsync(string bundleName, string assetName, bool active = false, Action<GameObject> action = null)
        {
            throw new NotImplementedException();
        }
    }
}
