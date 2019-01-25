using Core.Components;
using System.Collections.Generic;
using Common.EventLogger;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.Components.Player
{
    [Player]
    public class RecycleableAssetComponent : IAssetComponent
    {
        private static readonly LoggerAdapter ResLogger = new LoggerAdapter(EventLoggerConstant.Res);
        public GameObject PlayerRoot;
        
        private Dictionary<int, UnityObjectWrapper<GameObject>> _goCollection = new Dictionary<int, UnityObjectWrapper<GameObject>>();
        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerRecycleableAsset;
        }

        public void Recycle(IGameObjectPool gameObjectPool)
        {
            foreach (var go in _goCollection)
            {
                gameObjectPool.Add(go.Value);
            }
            _goCollection.Clear();

            // gameoobject through new when player loaded
            Object.DestroyImmediate(PlayerRoot);
        }

        public void Add(UnityObjectWrapper<GameObject> asset)
        {
            if (_goCollection.ContainsKey(asset.Id))
            {
                ResLogger.ErrorFormat("duplicate asset id: {0} address: {1}", asset.Id, asset.Address);
                return;
            }
            _goCollection.Add(asset.Id, asset);
        }

        public void Remove(UnityObjectWrapper<GameObject> asset)
        {
            _goCollection.Remove(asset.Id);
        }
    }
}