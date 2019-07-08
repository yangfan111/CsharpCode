using Assets.UiFramework.Libs;
using Core.Utils;
using System;
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
            LoadAsync(assetInfo, (obj) =>
            {
                if (null != callback)
                {
                    callback(GetSprite(obj as Sprite));
                }
            },objectType: typeof(Sprite));
        }

        public void UnLoad(string bundleName, string assetName, object source)
        {
            _assetManager.LoadCancel(source);
        }

        public void LoadAsync(string bundle, string name, Action<UnityEngine.Object> callback, GameObject parent = null)
        {
            LoadAsync(new AssetInfo(bundle, name), callback, parent);
        }

        public void LoadAsync(AssetInfo assetInfo, Action<UnityEngine.Object> callback, GameObject parent = null, Type objectType = null)
        {
            if (string.IsNullOrEmpty(assetInfo.BundleName) || string.IsNullOrEmpty(assetInfo.AssetName))
            {
                //Debug.LogError("asset error." + assetInfo.BundleName + "," + assetInfo.AssetName);
                return;
            }

            _assetManager.LoadAssetAsync("UiResourceLoader", assetInfo, (source, unityObj)=>
            {
                try
                {
                    if (callback != null)
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
            }, new AssetLoadOption(parent:_uiLoaderRoot,  recyclable:true , objectType: objectType));
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
            LoadAsync(bundleName, assetName, (obj) =>
            {
                if (null != callback)
                {
                    callback(obj as TextAsset);
                }
            });
        }

        private Sprite GetSprite(Sprite sprite)
        {
            if (null != sprite)
            {
                if (sprite.texture != null)
                {
                    if (sprite.texture.width >= 1024 && sprite.texture.height >= 512)
                    {
                        Logger.WarnFormat("You Would Better Change {0} Mode From Sprite To Default", sprite.name);
                    }
                }
                return sprite;
            }
            return ViewModelUtil.EmptySprite;
        }

        public void RetriveTextureAsync(string bundle, string asset, Action<Texture> callback)
        {
            LoadAsync(new AssetInfo(bundle, asset), (obj) =>
            {
                if (null != callback)
                {
                    callback(GetTexture(obj as Texture));
                }
            }, null, typeof(Texture));
        }

        private Texture GetTexture(Texture texture)
        {
            if (null != texture)
            {
                return texture;
            }
            return ViewModelUtil.EmptySprite.texture;
        }

        public void LoadUIEffectAsync(string bundleName, string assetName, bool active = false, Action<GameObject> action = null)
        {
            throw new NotImplementedException();
        }

        public void Load(GameObject prefab, string bundle, string name, Action<UnityEngine.Object> callback, GameObject parent = null)
        {
            throw new NotImplementedException();
        }
    }
}
