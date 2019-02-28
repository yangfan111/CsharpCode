using App.Shared;
using App.Shared.GameModules.ClientEffect;
using Utils.AssetManager;
using Core.Configuration;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal abstract class EnvironmentHitClientEffect : AbstractClientEffect
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(EnvironmentHitClientEffect));

        public override void OnCreate(EClientEffectType type, int effectId)
        {
            _effectConfig = SingletonManager.Get<ClientEffectConfigManager>().GetConfigItemByType(type);
            if(null != _effectConfig)
            {
                if(null != _effectConfig.Asset)
                {
                    Asset = new AssetInfo(_effectConfig.Asset.BundleName, _effectConfig.Asset.AssetName);
                }
            }
        }

        protected abstract EClientEffectType EffectType { get; }

        public override void Initialize(ClientEffectEntity entity)
        {
            var offset = 0.001f;
            var normal = entity.normal.Value;
            var go = (GameObject)entity.assets.FirstAsset;
            go.transform.position = entity.position.Value + offset * normal;
            go.transform.rotation = Quaternion.FromToRotation(Vector3.forward, normal);
            go.transform.localScale = Ssjj2AssetsUtility.LocalScale;
            if(entity.hasAttachParent)
            {
                switch((EEntityType)entity.attachParent.ParentKey.EntityType)
                {
                    case EEntityType.Vehicle:
                        var vehicle = AllContexts.vehicle.GetEntityWithEntityKey(entity.attachParent.ParentKey);
                        if(vehicle.hasGameObject && null != vehicle.gameObject.UnityObject)
                        {
                            go.transform.parent = vehicle.gameObject.UnityObject.AsGameObject.transform;
                            go.transform.position = vehicle.position.Value + entity.attachParent.Offset;
                        }
                        else
                        {
                            Logger.Error("vehicle gameobject does not exist");
                        }
                        break;
                    case EEntityType.Player:
                        var player = AllContexts.player.GetEntityWithEntityKey(entity.attachParent.ParentKey);
                        if(player.hasBones && null != player.bones.Spine)
                        {
                            go.transform.parent = player.bones.Spine.transform;
                            go.transform.position = player.position.Value + entity.attachParent.Offset;
                        }
                        else
                        {
                            Logger.Error("player sprine bones doesn't exist !");
                        }
                        break;
                    case EEntityType.SceneObject:
                        var trunkId = entity.attachParent.FragmentId;
                        var sceneObj = AllContexts.sceneObject.GetEntityWithEntityKey(entity.attachParent.ParentKey);
                        if(null == sceneObj)
                        {
                            entity.isFlagDestroy = true;
                        }
                        else
                        {
                            if(sceneObj.hasRawGameObject && null != sceneObj.rawGameObject)
                            {
                                var fracturedObject = sceneObj.rawGameObject.Value.GetComponent<FracturedObject>();
                                if(trunkId > -1 && trunkId < fracturedObject.ListFracturedChunks.Count)
                                {
                                    var trunk = fracturedObject.ListFracturedChunks[trunkId];
                                    if(null != trunk)
                                    {
                                        go.transform.parent = trunk.transform;
                                        go.transform.position = trunk.transform.position + entity.attachParent.Offset + normal * 0.01f;
                                    }
                                }
                            }
                            else
                            {
                                Logger.Error("target scene object has no rawgameobject");
                            }
                        }
                        break;
                }
            }
        }
    }

    internal class StoneHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.StoneHit; }
        }
    }

    internal class SteelHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.SteelHit; }
        }
    }

    internal class SoilHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.SoilHit; }
        }
    }

    internal class WoodHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.WoodHit; }
        }
    }

    internal class WaterHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.WaterHit; }
        }
    }

    internal class GlassHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.GlassHit; }
        }
    }

    internal class DefaultHitClientEffect : EnvironmentHitClientEffect
    {
        protected override EClientEffectType EffectType
        {
            get { return EClientEffectType.DefaultHit; }
        }
    }
}