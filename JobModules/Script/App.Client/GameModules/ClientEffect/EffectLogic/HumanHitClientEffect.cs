using App.Shared.Components.ClientEffect;
using Utils.AssetManager;
using UnityEngine;
using Core.Configuration;
using Core.Utils;
using Utils.Singleton;
using XmlConfig;
using Entitas;

namespace App.Shared.GameModules.ClientEffect
{
    public class HumanHitClientEffect : IEffectLogic
    {
        private Contexts _contexts;

        public void OnCreate(EClientEffectType type, int effectId)
        {
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(HumanHitClientEffect));
        public AssetInfo[] AssetInfos {
            get {
                var effect = SingletonManager.Get<ClientEffectConfigManager>().GetConfigItemByType(EClientEffectType.HumanHitEffect);
                if (null != effect)
                {
                    return new[] { new AssetInfo(effect.Asset.BundleName, effect.Asset.AssetName) };
                }
                return new AssetInfo[]{};
            }
        }

        public int SoundId
        {
            get
            {
                var effect = SingletonManager.Get<ClientEffectConfigManager>().GetConfigItemByType(EClientEffectType.HumanHitEffect);
                if(null != effect)
                {
                    return effect.SoundId;
                }
                return 0;
            }
        }

        public void Render(ClientEffectEntity entity)
        {
        }

        public void Initialize(ClientEffectEntity entity)
        {
            var go = (GameObject)entity.assets.FirstAsset;
            go.transform.localScale = Ssjj2AssetsUtility.LocalScale;
            go.transform.position = entity.position.Value;
            if (entity.hasAttachParent)
            {
                var player = _contexts.player.GetEntityWithEntityKey(entity.attachParent.ParentKey);
                if(null != player && player.hasBones)
                {
                    go.transform.parent = player.bones.Spine;
                    go.transform.position = player.position.Value + entity.attachParent.Offset;
                }
                else
                {
                    go.SetActive(false);
                    Logger.ErrorFormat("no player with key {0}, or the player has no bones", entity.attachParent);
                }
            }
        }

        public void SetContexts(IContexts contexts)
        {
            _contexts = contexts as Contexts;
        }
    }
}