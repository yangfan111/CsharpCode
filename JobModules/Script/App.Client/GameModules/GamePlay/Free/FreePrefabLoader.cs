using Assets.Sources.Free.UI;
using Utils.AssetManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using Utils.Appearance;

namespace App.Client.GameModules.GamePlay.Free
{
    public class FreePrefabLoader
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FreePrefabLoader));

        private static FreePrefabLoader freeHandler = new FreePrefabLoader();
        private static CachePrefabHandler cacheHandler = new CachePrefabHandler();

        private static Dictionary<string, Dictionary<string, Queue<GameObject>>> cache = new Dictionary<string, Dictionary<string, Queue<GameObject>>>();

        public static void Load(string assetBuddle, string assetName, Action<GameObject> action, GameObject parent = null)
        {
            if (cache.ContainsKey(assetBuddle) && cache[assetBuddle].ContainsKey(assetName) && cache[assetBuddle][assetName].Count > 0)
            {
                Queue<GameObject> queue = cache[assetBuddle][assetName];
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                AppearanceUtils.EnableRender(obj);
                action(obj);
                //Debug.LogFormat("asset {0}/{1} cache count {2}", assetBuddle, assetName, queue.Count);
            }
            else
            {
                var assetManager = SingletonManager.Get<FreeUiManager>().Contexts1.session.commonSession.AssetManager;
                assetManager.LoadAssetAsync(action, new AssetInfo(assetBuddle, assetName), freeHandler.OnLoadSucc, new AssetLoadOption(parent: parent, recyclable: true));
            }
        }

        public static void CacheGameObject(AssetInfo info, int count)
        {
            for (int i = GetCacheCount(info.BundleName, info.AssetName); i < count; i++)
            {
                var assetManager = SingletonManager.Get<FreeUiManager>().Contexts1.session.commonSession.AssetManager;
                assetManager.LoadAssetAsync("FreePrefabLoader", info, cacheHandler.OnLoadSucc, new AssetLoadOption(recyclable: true));
            }
        }

        public static void Destroy()
        {
            if (SingletonManager.Get<FreeUiManager>().Contexts1 != null && SingletonManager.Get<FreeUiManager>().Contexts1.session != null && SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects != null)
            {
                var assetManager = SingletonManager.Get<FreeUiManager>().Contexts1.session.commonSession.AssetManager;

                foreach (string buddle in cache.Keys)
                {
                    foreach (Queue<GameObject> que in cache[buddle].Values)
                    {
                        foreach (GameObject obj in que)
                        {
                            var unityObj = UnityObject.GetUnityObject(obj);
                            assetManager.Recycle(unityObj);

                        }
                    }
                }
            }
            cache.Clear();
        }

        private static int GetCacheCount(string assetBuddle, string assetName)
        {
            if (cache.ContainsKey(assetBuddle) && cache[assetBuddle].ContainsKey(assetName))
            {
                return cache[assetBuddle][assetName].Count;
            }

            return 0;
        }

        public static void ReturnGameObject(GameObject obj, AssetInfo info)
        {
            if (!cache.ContainsKey(info.BundleName))
            {
                cache.Add(info.BundleName, new Dictionary<string, Queue<GameObject>>());
            }
            if (!cache[info.BundleName].ContainsKey(info.AssetName))
            {
                cache[info.BundleName].Add(info.AssetName, new Queue<GameObject>());
            }

            if (obj != null)
            {
                obj.SetActive(false);
                cache[info.BundleName][info.AssetName].Enqueue(obj);
            }
            else
            {
                _logger.ErrorFormat("Load {0} Failed", info.BundleName + "/" + info.AssetName);
            }

        }

        public void OnLoadSucc(Action<GameObject> action, UnityObject unityObj)
        {
            var obj = unityObj.AsGameObject;
            if (obj == null)
            {
                _logger.ErrorFormat("Load {0} Failed", unityObj.Address);
            }
            else
            {
                obj.SetActive(true);
            }

            AppearanceUtils.EnableRender((GameObject)obj);
            action(obj);
        }
    }

    class CachePrefabHandler
    {

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            FreePrefabLoader.ReturnGameObject(unityObj, unityObj.Address);
        }
    }
}
