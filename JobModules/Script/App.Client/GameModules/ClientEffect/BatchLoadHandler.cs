using Core.Utils;
using Utils.AssetManager;
using UnityEngine;

namespace App.Client.GameModules.ClientEffect
{
    public class BatchLoadHandler
    {
        public BatchLoadHandler(ClientEffectEntity entity, AssetInfo[] assetInfos)
        {
            _entity = entity;
            _assetInfos = assetInfos;
        }

        private ClientEffectEntity _entity;
        private AssetInfo[] _assetInfos;
     

        public void Load(IUnityAssetManager assetManager)
        {
            foreach (var assetInfo in _assetInfos)
            {
                assetManager.LoadAssetAsync("BatchLoadHandler", assetInfo, OnLoadSucc, new AssetLoadOption(dontAutoActive:true));
            }
        }
    private LoggerAdapter _adapter = new LoggerAdapter("BatchAsset");
        public void OnLoadSucc(string source, UnityObject obj)
        {
            if (_entity.hasAssets)
            {
            _entity.assets.LoadedAssets.Add(obj.Address, obj);
            bool isFinish = _entity.assets.LoadedAssets.Count == _assetInfos.Length;
            _entity.assets.IsLoadSucc = isFinish;
                
            }
            else
            {
                _adapter.Warn("ClientEffect entity  dont has Assets!");
            }
            
        }
    }
}