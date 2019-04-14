using App.Client.CastObjectUtil;
using App.Client.GameModules.Effect;
using App.Shared;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Appearance;
using Utils.Appearance.Weapon;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;
using AssetInfo = Utils.AssetManager.AssetInfo;

namespace App.Client.GameModules.SceneObject
{
    /// <summary>
    /// Defines the <see cref="SceneObjectLoadSystem" />
    /// </summary>
    public class SceneObjectLoadSystem : ReactiveResourceLoadSystem<SceneObjectEntity>
    {
        private readonly SimpleLoadRespondHander _simpleLoadHandler;

        private readonly LoadWeaponRespondHandler _weaponLoadHandler;

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectLoadSystem));

        private IUnityAssetManager _assetManager;

        private PlayerContext _playerContext;

        // private static readonly AssetInfo GroundProbFlashAsset = new AssetInfo("effect/common", "GroundPropFlash");
        public SceneObjectLoadSystem(Contexts contexts, ICommonSessionObjects sessionObjects) : base(
            contexts.sceneObject)
        {
            _assetManager      = sessionObjects.AssetManager;
            _playerContext     = contexts.player;
            _simpleLoadHandler = new SimpleLoadRespondHander(contexts);
            _weaponLoadHandler = new LoadWeaponRespondHandler(contexts);
        }

        protected override ICollector<SceneObjectEntity> GetTrigger(IContext<SceneObjectEntity> context)
        {
            return context.CreateCollector(SceneObjectMatcher.Position.Added());
        }

        protected override bool Filter(SceneObjectEntity entity)
        {
            return entity.hasSimpleEquipment || entity.hasWeaponObject;
        }

        public override void SingleExecute(SceneObjectEntity sceneObjectEntity)
        {
            if (!sceneObjectEntity.hasEffects)
                sceneObjectEntity.AddEffects();
            if (sceneObjectEntity.hasSimpleEquipment)
            {
                LoadSimpleEquip(sceneObjectEntity);
            }

            if (sceneObjectEntity.hasWeaponObject)
            {
                LoadWeapon(sceneObjectEntity);
            }
        }

        private void LoadWeapon(SceneObjectEntity sceneObjectEntity)
        {
            var weapon = sceneObjectEntity.weaponObject;
            //WeaponEntity weaponEntity = WeaponEntityFactory.StuffWeaponEntityFromSceneObject(weapon);
            //if (weaponEntity == null)
            //    return;
            var allConfigs = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.ConfigId);
            if (allConfigs == null)
                return;
            weapon.ApplyParts(allConfigs.DefaultParts);
            LoadWeaponResources(sceneObjectEntity, allConfigs.NewWeaponCfg);
        }

        private void LoadSimpleEquip(SceneObjectEntity sceneObjectEntity)
        {
            var category = sceneObjectEntity.simpleEquipment.Category;
            var id       = sceneObjectEntity.simpleEquipment.Id;
            if (id < 1)
            {
                Logger.ErrorFormat("itemid is illegal for weapon {0}", sceneObjectEntity.entityKey);
                return;
            }

            switch ((ECategory) category)
            {
                case ECategory.Weapon:
                    var allConfigs = SingletonManager
                                     .Get<WeaponConfigManagement>()
                                     .FindConfigById(sceneObjectEntity.weaponObject.ConfigId);
                    var weaponAvatarCfg = SingletonManager
                                          .Get<WeaponAvatarConfigManager>()
                                          .GetConfigById(allConfigs.NewWeaponCfg.AvatorId);
                    if (null == weaponAvatarCfg || allConfigs == null)
                    {
                        return;
                    }

                    if (weaponAvatarCfg.Size != 1)
                        sceneObjectEntity.AddSize(weaponAvatarCfg.Size);

                    sceneObjectEntity.weaponObject.ApplyParts(allConfigs.DefaultParts);
                    LoadWeaponResources(sceneObjectEntity, allConfigs.NewWeaponCfg);
                    break;
                case ECategory.Avatar:
                    var avatarCfg = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(id);
                    if (null != avatarCfg)
                    {
                        var player = _playerContext.flagSelfEntity;
                        if (null != player && player.hasPlayerInfo)
                        {
                            LoadAvatar(sceneObjectEntity, player, avatarCfg);
                        }
                    }

                    break;
                case ECategory.WeaponPart:
                    var partId    = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(id);
                    var partCfg   = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                    var partAsset = new AssetInfo(partCfg.Bundle, partCfg.Res);
                    if (string.IsNullOrEmpty(partAsset.AssetName) || string.IsNullOrEmpty(partAsset.BundleName))
                    {
                        break;
                        ;
                    }

                    var partSize = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetSizeById(id);
                    if (partSize != 1)
                    {
                        sceneObjectEntity.AddSize(partSize);
                    }

                    _assetManager.LoadAssetAsync(sceneObjectEntity, partAsset, _simpleLoadHandler.OnLoadSucc);

                    sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);

                    break;
                case ECategory.GameItem:
                    var itemSsset = SingletonManager.Get<GameItemConfigManager>().GetAssetById(id);
                    if (string.IsNullOrEmpty(itemSsset.AssetName) || string.IsNullOrEmpty(itemSsset.BundleName))
                    {
                        return;
                    }

                    var itemSize = SingletonManager.Get<GameItemConfigManager>().GetSizeById(id);
                    if (itemSize != 1)
                    {
                        sceneObjectEntity.AddSize(itemSize);
                    }

                    _assetManager.LoadAssetAsync(sceneObjectEntity, itemSsset, _simpleLoadHandler.OnLoadSucc);
                    sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
                    break;
            }
        }

        private void LoadWeaponResources(SceneObjectEntity                  sceneObjectEntity,
                                         WeaponConfigNs.WeaponResConfigItem weaponCfg)
        {
            var avatarId = weaponCfg.AvatorId;
            if (sceneObjectEntity.hasWeaponObject && sceneObjectEntity.weaponObject.WeaponAvatarId > 0)
            {
                avatarId = sceneObjectEntity.weaponObject.WeaponAvatarId;
            }

            AssetInfo asset =
                SingletonManager.Get<WeaponAvatarConfigManager>().GetThirdPersonModel(avatarId);
            bool alreadyInitializedBefore = true;
            if (!sceneObjectEntity.hasMultiUnityObject || !sceneObjectEntity.hasWeaponAttachment)
            {
                alreadyInitializedBefore = false;
                
                sceneObjectEntity.AddMultiUnityObject();
                sceneObjectEntity.AddWeaponAttachment(
                    new Dictionary<AssetInfo, int>(AssetInfo.AssetInfoComparer.Instance), new List<AssetInfo>());
            }

            var dic = sceneObjectEntity.multiUnityObject.LoadedAssets;
            if (!alreadyInitializedBefore)
            {
                AssetInfo assetInfo = new AssetInfo(asset.BundleName, asset.AssetName);
                dic[assetInfo] = null;
                _assetManager.LoadAssetAsync(sceneObjectEntity, assetInfo, _weaponLoadHandler.OnLoadSucc);
            }

            var weaponParts = sceneObjectEntity.weaponObject.CollectParts();
            for (int i = 0; i < weaponParts.Count; i++)
            {
                var       attachId  = weaponParts[i];
                var       attach    = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(attachId);
                AssetInfo partsInfo = new AssetInfo(attach.BundleName, attach.AssetName);
                if (!sceneObjectEntity.weaponAttachment.AttachmentDic.ContainsKey(partsInfo))
                {
                    dic[partsInfo] = null;
                    sceneObjectEntity.weaponAttachment.AttachmentDic[partsInfo] =
                        (int) SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(attachId);
                    _assetManager.LoadAssetAsync(sceneObjectEntity, partsInfo, _weaponLoadHandler.OnLoadSucc);
                }
            }

            List<AssetInfo> effectAssetInfo =
                SingletonManager.Get<WeaponAvatarConfigManager>().GetEffectAsset(avatarId);
            if (effectAssetInfo.Count > 0)
            {
                foreach (var effect in effectAssetInfo)
                {
                    dic[effect] = null;
                    _assetManager.LoadAssetAsync(sceneObjectEntity, effect, _weaponLoadHandler.OnLoadSucc);
                    sceneObjectEntity.weaponAttachment.EffectDic.Add(effect);
                }
            }

            sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
        }

        private void LoadAvatar(SceneObjectEntity    sceneObjectEntity, PlayerEntity playerEntity,
                                RoleAvatarConfigItem avatarCfg)
        {
            var role = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(playerEntity.playerInfo.RoleModelId);
            if (null != role)
            {
                switch ((Sex) role.Sex)
                {
                    case Sex.Female:
                        var fAvatarAsset = new AssetInfo(avatarCfg.Bundle, avatarCfg.FPrefab);
                        _assetManager.LoadAssetAsync(sceneObjectEntity, fAvatarAsset, _simpleLoadHandler.OnLoadSucc);
                        break;
                    case Sex.Male:
                        var mAvatarAsset = new AssetInfo(avatarCfg.Bundle, avatarCfg.MPrefab);
                        _assetManager.LoadAssetAsync(sceneObjectEntity, mAvatarAsset, _simpleLoadHandler.OnLoadSucc);
                        break;
                }

                sceneObjectEntity.effects.AddGlobalEffect(GlobalEffectManager.GlobalGroundPropFlash);
            }
        }

        /// <summary>
        /// Defines the <see cref="SimpleLoadRespondHander" />
        /// </summary>
        private class SimpleLoadRespondHander : ResponseHandler
        {
            public void OnLoadSucc(SceneObjectEntity entity, UnityObject unityObj)
            {
                if (null == entity)
                {
                    Logger.Error("Entity type mismatched !!");
                    return;
                }

                var model = unityObj.AsGameObject;
                if (null == model)
                {
                    Logger.Error("Resource load type mismatched");
                    return;
                }

                else
                {
                    entity.AddUnityObject(unityObj);
                }

                AppearanceUtils.EnableRender(model);
                InitSceneObject(entity, model);
            }

            public SimpleLoadRespondHander(Contexts contexts) : base(contexts)
            {
            }
        }

        /// <summary>
        /// Defines the <see cref="LoadWeaponRespondHandler" />
        /// </summary>
        private class LoadWeaponRespondHandler : ResponseHandler
        {
            private BoneMount _boneMount;

            public LoadWeaponRespondHandler(Contexts contexts) : base(contexts)
            {
                _boneMount = new BoneMount();
            }

            public void OnLoadSucc(SceneObjectEntity entity, UnityObject unityObj)
            {
                if (null == entity)
                {
                    Logger.Error("Entity type mismatched !!");
                    return;
                }

                var model = unityObj.AsGameObject;
                if (null == model)
                {
                    Logger.Error("Resource load type mismatched");
                    return;
                }

                if (!entity.hasMultiUnityObject)
                {
                    Logger.Error("MultiUnityObject component is needed !");
                    return;
                }

                if (entity.multiUnityObject.LoadedAssets.ContainsKey(unityObj.Address))
                {
                    entity.multiUnityObject.LoadedAssets[unityObj.Address] = unityObj;
                    AppearanceUtils.EnableRender(model);
                }
                else
                {
                    Logger.ErrorFormat("asset {0} should not be treated as multi asset ", unityObj.Address);
                }

                bool finished = true;
                foreach (var loaded in entity.multiUnityObject.LoadedAssets)
                {
                    if (null == loaded.Value)
                    {
                        finished = false;
                    }
                }

                if (!finished)
                {
                    return;
                }

                AssembleWeapon(entity);
                // 完成
                InitSceneObject(entity, entity.multiUnityObject.FirstAsset);
            }

            private void AssembleWeapon(SceneObjectEntity entity)
            {
                var weaponGo = entity.multiUnityObject.FirstAsset;
                if (entity.hasWeaponAttachment)
                {
                    var dic = entity.weaponAttachment.AttachmentDic;
                    foreach (var pair in dic)
                    {
                        var attachGo = entity.multiUnityObject.LoadedAssets[pair.Key];
                        var location = WeaponPartLocation.EndOfTheWorld;
                        location = WeaponTypeConvertUtil.GetLocationByPartType((EWeaponPartType) pair.Value);
                        if (location != WeaponPartLocation.EndOfTheWorld)
                        {
                            _boneMount.MountWeaponAttachment(attachGo, weaponGo, location);
                        }
                        else
                        {
                            Logger.ErrorFormat("Location is illegal with item type {0}", pair.Value);
                        }
                    }

                    var effectDic = entity.weaponAttachment.EffectDic;
                    if (effectDic.Count > 0)
                    {
                        foreach (var effect in effectDic)
                        {
                            var attachGo = entity.multiUnityObject.LoadedAssets[effect];
                            attachGo.AsGameObject.transform.position = entity.position.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Defines the <see cref="ResponseHandler" />
        /// </summary>
        private class ResponseHandler
        {
            private readonly Contexts _contexts;

            public ResponseHandler(Contexts contexts)
            {
                _contexts = contexts;
            }

            private void EnableAutoPickup(int entityId, GameObject normalCollider)
            {
                var listener = normalCollider.AddComponent<SceneObjectTriggerEnterListener>();
                listener.SetEntityId(entityId);
            }

            protected void InitSceneObject(SceneObjectEntity entity, GameObject model)
            {
                var target = SceneObjectGoAssemble.Assemble(model, entity);
                if (entity.hasSize)
                {
                    var size = Mathf.Max(1f, entity.size.Value);
                    model.transform.localScale  = Vector3.one * entity.size.Value;
                    target.transform.localScale = Vector3.one * size;
                }

                if (entity.hasEffects)
                {
                    foreach (var effect in entity.effects.GlobalEffects)
                    {
                        _contexts.session.clientSessionObjects.GlobalEffectManager.AddGameObject(effect, model);
                    }
                }

                if (entity.hasSimpleEquipment)
                {
                    ProcessSimpleEquipment(entity, target);
                }
                else if (entity.hasWeaponObject)
                {
                    ProcessWeapon(entity, target);
                }
            }

            private void ProcessSimpleEquipment(SceneObjectEntity entity, RayCastTarget target)
            {
                var equip    = entity.simpleEquipment;
                var category = entity.simpleEquipment.Category;
                var entityId = entity.entityKey.Value.EntityId;
                SceneObjCastData.Make(target, entityId, equip.Id, equip.Count, category);
                switch ((ECategory) category)
                {
                    case ECategory.Weapon:
                        var weaponCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(equip.Id);
                        if (null == weaponCfg)
                        {
                            break;
                        }

                        var weaponType = (EWeaponType_Config) weaponCfg.Type;
                        if (weaponType.CanAutoPick())
                        {
                            EnableAutoPickup(entityId, target.gameObject);
                        }

                        break;
                    default:
                        break;
                }
            }

            private void ProcessWeapon(SceneObjectEntity entity, RayCastTarget target)
            {
                var weapon   = entity.weaponObject;
                var entityId = entity.entityKey.Value.EntityId;
                SceneObjCastData.Make(target, entityId, weapon.ConfigId, 1, (int) ECategory.Weapon);
                var weaponType = (EWeaponType_Config) SingletonManager
                                                      .Get<WeaponResourceConfigManager>().GetConfigById(weapon.ConfigId)
                                                      .Type;
                if (weaponType.CanAutoPick())
                {
                    EnableAutoPickup(entityId, target.gameObject);
                }
            }
        }
    }
}