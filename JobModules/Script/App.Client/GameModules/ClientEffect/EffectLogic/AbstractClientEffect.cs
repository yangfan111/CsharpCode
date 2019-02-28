using App.Shared.Components.ClientEffect;
using Core.Configuration;
using Core.IFactory;
using UnityEngine;
using XmlConfig;
using Utils.AssetManager;
using Utils.Singleton;
using Entitas;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal abstract class AbstractClientEffect : IEffectLogic
    {
        protected Contexts AllContexts;
        protected ClientEffectConfigItem _effectConfig;
        protected ISoundEntityFactory _soundEntityFactory;  

        public virtual void OnCreate(EClientEffectType type, int effectId)
        {
            _effectConfig = SingletonManager.Get<ClientEffectConfigManager>().GetConfigItemById(effectId);
            if(null != _effectConfig)
            {
                if(null != _effectConfig.Asset)
                {
                    Asset = new AssetInfo(_effectConfig.Asset.BundleName, _effectConfig.Asset.AssetName);
                   
                }
            }
        }

        protected AssetInfo Asset;
        private AssetInfo[] _assetInfos = new AssetInfo[1];
        public AssetInfo[] AssetInfos
        {
            get
            {
                _assetInfos[0] = Asset;
                return _assetInfos;
            }
        }

        public virtual int SoundId
        {
            get
            {
                if(null != _effectConfig)
                {
                    return _effectConfig.SoundId;
                }
                return 0;
            }
        }

        public virtual void Render(ClientEffectEntity entity)
        {

        }

        public virtual void Initialize(ClientEffectEntity entity)
        {
            var go = (GameObject)entity.assets.LoadedAssets[Asset];

            var position = entity.position.Value;
            if (entity.hasEffectRotation)
            {
                var yaw = entity.effectRotation.Yaw;
            }
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        public void SetContexts(IContexts contexts)
        {
            AllContexts = contexts as Contexts;
            _soundEntityFactory = AllContexts.session.entityFactoryObject.SoundEntityFactory;
        }
    }
}
