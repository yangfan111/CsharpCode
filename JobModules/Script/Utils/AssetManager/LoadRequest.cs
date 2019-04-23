using System;
using UnityEngine;
using Utils.AssetManager;

namespace Utils.AssetManager
{
    public interface ILoadedHandler
    {
        void OnLoadSucc<T>(T source, UnityObject unityObj);
    }

    public class LoadRequestFactory
    {
        public static AbstractLoadRequest Create<T>(AssetInfo assetInfo, Action<T, UnityObject> handler) where T: class
        {
            return new LoadRequest<T>(assetInfo, handler);
        }
    }

    public abstract class AbstractLoadRequest
    {
        public AssetInfo AssetInfo;

        public AbstractLoadRequest(AssetInfo assetInfo)
        {
            AssetInfo = assetInfo;
        }

        public override string ToString()
        {
            return string.Format("AssetInfo: {0}", AssetInfo);
        }

        public abstract Action<TSource, UnityObject> GetHandler<TSource>() where TSource: class;
    }

    public class LoadRequest<T> : AbstractLoadRequest where T: class
    {
        public LoadRequest(AssetInfo assetInfo, Action<T, UnityObject> handler): base(assetInfo)
        {
            _handler = handler;
        }

        private Action<T, UnityObject> _handler;

        public override Action<TSource, UnityObject> GetHandler<TSource>()
        {
            return  (source, unityObj) =>
            {
                _handler(source as T, unityObj);
            };
        }
    }
}