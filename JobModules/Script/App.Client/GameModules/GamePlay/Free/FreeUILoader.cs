using App.Client.GameModules.Free;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Utils.AssetManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utils.Singleton;

namespace Assets.App.Client.GameModules.GamePlay.Free
{
    public class FreeUILoader
    {
        private static FreeUILoader freeHandler = new FreeUILoader();
        private static CacheUIHandler cacheHandler = new CacheUIHandler();
        private static Dictionary<string, Dictionary<string, Queue<UnityEngine.Object>>> cache = new Dictionary<string, Dictionary<string, Queue<UnityEngine.Object>>>();

        public static void Load(object source, string assetBuddle, string assetName)
        {
            var assetManager = SingletonManager.Get<FreeUiManager>().Contexts1.session.commonSession.AssetManager;

            if (cache.ContainsKey(assetBuddle) && cache[assetBuddle].ContainsKey(assetName) && cache[assetBuddle][assetName].Count > 0)
            {
                Queue<UnityEngine.Object> queue = cache[assetBuddle][assetName];
                SetTexture(source, queue.Dequeue());
                Debug.LogFormat("asset {0}/{1} cache count {2}", assetBuddle, assetName, queue.Count);
            }
            else
            {
                assetManager.LoadAssetAsync(source, new AssetInfo(assetBuddle, assetName), freeHandler.OnLoadSucc);
                Debug.LogFormat("asset {0}/{1} cache count 0", assetBuddle, assetName);
            }
        }

        private static void SetTexture(object source, UnityEngine.Object obj)
        {
            if (source is RawImage)
            {
                ((RawImage)source).texture = ((Texture)obj);
            }
            if (source is Image)
            {
                ((Image)source).sprite = (Sprite)obj;
            }
        }

        public static void Destroy()
        {
            cache.Clear();
        }

        public static void CacheGameObject(AssetInfo info, int count)
        {
            for (int i = GetCacheCount(info.BundleName, info.AssetName); i < count; i++)
            {
                var assetManager = SingletonManager.Get<FreeUiManager>().Contexts1.session.commonSession.AssetManager;
                assetManager.LoadAssetAsync("FreeUILoader", info, cacheHandler.OnLoadSucc);
            }
        }

        private static int GetCacheCount(string assetBuddle, string assetName)
        {
            if (cache.ContainsKey(assetBuddle) && cache[assetBuddle].ContainsKey(assetName))
            {
                return cache[assetBuddle][assetName].Count;
            }

            return 0;
        }

        public static void ReturnGameObject(UnityEngine.Object obj, AssetInfo info)
        {
            if (!cache.ContainsKey(info.BundleName))
            {
                cache.Add(info.BundleName, new Dictionary<string, Queue<UnityEngine.Object>>());
            }
            if (!cache[info.BundleName].ContainsKey(info.AssetName))
            {
                cache[info.BundleName].Add(info.AssetName, new Queue<UnityEngine.Object>());
            }
            cache[info.BundleName][info.AssetName].Enqueue(obj);
        }

        public void OnLoadSucc(object source, UnityObject unityObj)
        {
            var assetInfo = unityObj.Address;
            if (source is Image)
            {
                Texture2D tex = unityObj.As<Texture2D>();
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect);
                ReturnGameObject(sprite, assetInfo);
            }
            else if (source is RawImage)
            {
                ReturnGameObject(unityObj.As<Texture>(), assetInfo);
            }

            SetTexture(source, cache[assetInfo.BundleName][assetInfo.AssetName].Dequeue());
        }
    }

    class CacheUIHandler
    {

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            var obj = unityObj.AsObject;
            var assetInfo = unityObj.Address;
            if (obj is Texture2D)
            {
                Texture2D tex = (Texture2D)obj;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect);
                FreeUILoader.ReturnGameObject(sprite, assetInfo);
            }
            else if (obj is Texture)
            {
                FreeUILoader.ReturnGameObject((Texture)obj, assetInfo);
            }
        }
    }
}
